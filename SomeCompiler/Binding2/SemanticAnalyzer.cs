using SomeCompiler.Parser.Model;

namespace SomeCompiler.Binding2;

public class SemanticAnalyzer
{
    public AnalyzeResult<SemanticNode> Analyze(Program program) => AnalyzeProgram(program, Scope.Empty);

    public AnalyzeResult<SemanticNode> AnalyzeProgram(Program node, Scope scope)
    {
        var functions = new List<FunctionNode>();
        foreach (var statement in node.Functions)
        {
            var functionResult = AnalyzeFunction(statement, scope);
            scope = functionResult.Scope;
            functions.Add(functionResult.Node);
        }

        return new AnalyzeResult<SemanticNode>(new ProgramNode(functions), scope);
    }

    private AnalyzeResult<FunctionNode> AnalyzeFunction(Function function, Scope parentScope)
    {
        var analyzeBlockResult = AnalyzeBlock(function.Block, parentScope);
        return new AnalyzeResult<FunctionNode>(new FunctionNode(function.Name, analyzeBlockResult.Node), analyzeBlockResult.Scope);
    }

    private AnalyzeResult<BlockNode> AnalyzeBlock(Block block, Scope scope)
    {
        var statements = new List<StatementNode>();
        foreach (var statement in block)
        {
            var analyzedStatementResult = AnalyzeStatement(statement, scope);
            statements.Add(analyzedStatementResult.Node);
            scope = analyzedStatementResult.Scope;
        }

        return new AnalyzeResult<BlockNode>(new BlockNode(statements), scope);
    }

    private AnalyzeResult<StatementNode> AnalyzeStatement(Statement statement, Scope scope)
    {
        switch (statement)
        {
            case DeclarationStatement declarationStatement:
                return AnalyzeDeclaration(declarationStatement, scope);
            case ExpressionStatement expressionStatement:
                return AnalyzeExpressionStatement(expressionStatement, scope);
            case IfElseStatement ifElseStatement:
                break;
            case ReturnStatement returnStatement:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(statement));
        }

        throw new InvalidOperationException();
    }

    private AnalyzeResult<StatementNode> AnalyzeExpressionStatement(ExpressionStatement expressionStatement, Scope scope)
    {
        var analyzeExpressionResult = AnalyzeExpression(expressionStatement.Expression, scope);
        return new AnalyzeResult<StatementNode>(new ExpressionStatementNode(analyzeExpressionResult.Node), scope);
    }

    private AnalyzeResult<ExpressionNode> AnalyzeExpression(Expression expression, Scope scope)
    {
        if (expression is AssignmentExpression assignmentExpression)
        {
            return AnalyzeAssignmentExpression(scope, assignmentExpression);
        }

        if (expression is ConstantExpression c)
        {
            return new AnalyzeResult<ExpressionNode>(new ConstantNode(c.Value), scope);
        }

        if (expression is BinaryExpression binaryExpression)
        {
            return AnalyzeBinaryExpression(binaryExpression, scope);
        }

        if (expression is IdentifierExpression i)
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
        return scope.Get(name).Match(symbol => (SymbolNode) new KnownSymbolNode(symbol), () => new UnknownSymbol(name));
    }

    private AnalyzeResult<ExpressionNode> AnalyzeBinaryExpression(BinaryExpression binaryExpression, Scope scope)
    {
        if (binaryExpression is AddExpression)
        {
            var left = AnalyzeExpression(binaryExpression.Left, scope);
            var right = AnalyzeExpression(binaryExpression.Right, scope);
            return new AnalyzeResult<ExpressionNode>(new AddExpressionNode(left.Node, right.Node), scope);
        }

        throw new InvalidOperationException("Por aquí no vas a ningún sitio tampoco");
    }

    private AnalyzeResult<ExpressionNode> AnalyzeAssignmentExpression(Scope scope, AssignmentExpression assignmentExpression)
    {
        var symbolNode = GetSymbolNode(scope, assignmentExpression.Left.Identifier);

        var analyzeExpression = AnalyzeExpression(assignmentExpression.Right, scope);

        return new AnalyzeResult<ExpressionNode>(new AssignmentNode(symbolNode, analyzeExpression.Node)
        {
            Errors = SymbolError(symbolNode)
        }, scope);
    }

    private AnalyzeResult<StatementNode> AnalyzeDeclaration(DeclarationStatement declarationStatement, Scope scope)
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