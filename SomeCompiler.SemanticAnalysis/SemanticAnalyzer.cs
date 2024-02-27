using SomeCompiler.Parser;

namespace SomeCompiler.SemanticAnalysis;

public class SemanticAnalyzer
{
    public AnalyzeResult<SemanticNode> Analyze(ProgramSyntax program) => AnalyzeProgram(program, Scope.Empty);

    private AnalyzeResult<SemanticNode> AnalyzeProgram(ProgramSyntax node, Scope scope)
    {
        var functions = new List<FunctionNode>();
        foreach (var function in node.Functions)
        {
            var functionResult = AnalyzeFunction(function, scope);
            scope = functionResult.Scope;
            functions.Add(functionResult.Node);
        }

        return new AnalyzeResult<SemanticNode>(new ProgramNode(functions), scope);
    }

    private AnalyzeResult<FunctionNode> AnalyzeFunction(FunctionSyntax function, Scope parentScope)
    {
        var analyzeBlockResult = AnalyzeBlock(function.Block, parentScope);
        return new AnalyzeResult<FunctionNode>(new FunctionNode(function.Name, analyzeBlockResult.Node), analyzeBlockResult.Scope);
    }

    private AnalyzeResult<BlockNode> AnalyzeBlock(BlockSyntax block, Scope scope)
    {
        var statements = new List<StatementNode>();
        foreach (var statement in block.Statements)
        {
            var analyzedStatementResult = AnalyzeStatement(statement, scope);
            statements.Add(analyzedStatementResult.Node);
            scope = analyzedStatementResult.Scope;
        }

        return new AnalyzeResult<BlockNode>(new BlockNode(statements), scope);
    }

    private AnalyzeResult<StatementNode> AnalyzeStatement(StatementSyntax statement, Scope scope)
    {
        switch (statement)
        {
            case DeclarationSyntax declarationStatement:
                return AnalyzeDeclaration(declarationStatement, scope);
            case ExpressionStatementSyntax expressionStatement:
                return AnalyzeExpressionStatement(expressionStatement, scope);
            case IfElseSyntax ifElseStatement:
                break;
            case ReturnSyntax returnStatement:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(statement));
        }

        throw new InvalidOperationException();
    }

    private AnalyzeResult<StatementNode> AnalyzeExpressionStatement(ExpressionStatementSyntax expressionStatement, Scope scope)
    {
        var analyzeExpressionResult = AnalyzeExpression(expressionStatement.Expression, scope);
        return new AnalyzeResult<StatementNode>(new ExpressionStatementNode(analyzeExpressionResult.Node), scope);
    }

    private AnalyzeResult<ExpressionNode> AnalyzeExpression(ExpressionSyntax expression, Scope scope)
    {
        if (expression is ConstantSyntax c)
        {
            return new AnalyzeResult<ExpressionNode>(new ConstantNode(c.Value), scope);
        }

        if (expression is BinaryExpressionSyntax binaryExpression)
        {
            return AnalyzeBinaryExpression(binaryExpression, scope);
        }

        if (expression is IdentifierSyntax i)
        {
            var symbolNode = GetSymbolNode(scope, i.Identifier);
            var symbolExpressionNode = new SymbolExpressionNode(symbolNode)
            {
                Errors = SymbolError(symbolNode)
            };
            return new AnalyzeResult<ExpressionNode>(symbolExpressionNode, scope);
        }

        throw new InvalidOperationException("Por aquí no vas a ningún sitio");
    }

    private IEnumerable<string> SymbolError(SymbolNode symbolNode)
    {
        return CheckSymbol(symbolNode).Map(s => new List<string> { s }).GetValueOrDefault([]);
    }

    private Maybe<string> CheckSymbol(SymbolNode symbolNode)
    {
        if (symbolNode is UnknownSymbol)
        {
            return $"Use of undeclared variable '{symbolNode}'";
        }

        return Maybe<string>.None;
    }

    private SymbolNode GetSymbolNode(Scope scope, string name)
    {
        return scope.Get(name).Match(symbol => (SymbolNode)new KnownSymbolNode(symbol), () => new UnknownSymbol(name));
    }

    private AnalyzeResult<ExpressionNode> AnalyzeBinaryExpression(BinaryExpressionSyntax binaryExpression, Scope scope)
    {
        if (binaryExpression is AddExpression)
        {
            var left = AnalyzeExpression(binaryExpression.Left, scope);
            var right = AnalyzeExpression(binaryExpression.Right, scope);
            return new AnalyzeResult<ExpressionNode>(new AddExpressionNode(left.Node, right.Node), scope);
        }

        throw new InvalidOperationException("Por aquí no vas a ningún sitio tampoco");
    }

    //private AnalyzeResult<ExpressionNode> AnalyzeAssignmentExpression(Scope scope, AssignmentExpression assignmentExpression)
    //{
    //    var symbolNode = GetSymbolNode(scope, assignmentExpression.Left.Identifier);

    //    var analyzeExpression = AnalyzeExpression(assignmentExpression.Right, scope);

    //    return new AnalyzeResult<ExpressionNode>(new AssignmentNode(symbolNode, analyzeExpression.Node)
    //    {
    //        Errors = SymbolError(symbolNode)
    //    }, scope);
    //}

    private AnalyzeResult<StatementNode> AnalyzeDeclaration(DeclarationSyntax declarationStatement, Scope scope)
    {
        var declaration = scope
            .TryDeclare(new Symbol(declarationStatement.Name, IntType.Instance))
            .Match(
                s => new AnalyzeResult<StatementNode>(new DeclarationNode(declarationStatement.Name, s), s),
                _ => new AnalyzeResult<StatementNode>(new DeclarationNode(declarationStatement.Name, scope)
                {
                    Errors = [$"Variable {declarationStatement.Name} is already declared"]
                }, scope));
        return declaration;
    }
}