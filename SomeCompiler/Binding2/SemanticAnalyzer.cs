using SomeCompiler.Parser.Model;

namespace SomeCompiler.Binding2;

public class SemanticAnalyzer
{
    public SemanticNode Analyze(Program program)
    {
        return AnalyzeProgram(program, Scope.Empty).Item1;
    }

    public (SemanticNode, Scope) AnalyzeProgram(Program node, Scope scope)
    {
        var functions = new List<FunctionNode>();
        foreach (var statement in node.Functions)
        {
            var (functionNode, newScope) = AnalyzeFunction(statement, scope);
            scope = newScope;
            functions.Add(functionNode);
        }
        return (new ProgramNode(functions), scope);
    }

    private (FunctionNode, Scope) AnalyzeFunction(Function function, Scope parentScope)
    {
        var analyzeBlock = AnalyzeBlock(function.Block, parentScope);
        return (new FunctionNode(function.Name, analyzeBlock.Item1), analyzeBlock.Item2);
    }

    private (BlockNode, Scope) AnalyzeBlock(Block block, Scope scope)
    {
        var statements = new List<StatementNode>();
        foreach (var statement in block)
        {
            var (analyzedStatement, newScope) = AnalyzeStatement(statement, scope);
            statements.Add(analyzedStatement);
            scope = newScope;
        }
        return (new BlockNode(statements), scope);
    }


    private (StatementNode, Scope) AnalyzeStatement(Statement statement, Scope scope)
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

    private (StatementNode, Scope) AnalyzeExpressionStatement(ExpressionStatement expressionStatement, Scope scope)
    {
        var analyzeExpression = AnalyzeExpression(expressionStatement.Expression, scope);
        return (new ExpressionStatementNode(analyzeExpression.Item1), scope);
    }

    private (ExpressionNode, Scope) AnalyzeExpression(Expression expression, Scope scope)
    {
        if (expression is AssignmentExpression assignmentExpression)
        {
            return AnalyzeAssignmentExpression(scope, assignmentExpression);
        }
        else if (expression is ConstantExpression c)
        {
            return (new ConstantNode(c.Value), scope);
        }

        if (expression is BinaryExpression binaryExpression)
        {
            return AnalyzeBinaryExpression(binaryExpression, scope);
        }

        if (expression is IdentifierExpression i)
        {
            var symbolNode = GetSymbolNode(scope, i.Identifier);
            return (new SymbolExpressionNode(symbolNode), scope);
        }

        throw new InvalidOperationException("Por aquí no vas a ningún sitio");
    }

    private SymbolNode GetSymbolNode(Scope scope, string name)
    {
        return scope.Get(name).Match(symbol => (SymbolNode)new KnownSymbolNode(symbol), () => new UnknownSymbol(name));
    }

    private (ExpressionNode, Scope) AnalyzeBinaryExpression(BinaryExpression binaryExpression, Scope scope)
    {
        if (binaryExpression is AddExpression)
        {
            var left = AnalyzeExpression(binaryExpression.Left, scope);
            var right = AnalyzeExpression(binaryExpression.Right, scope);
            return (new AddExpressionNode(left.Item1, right.Item1), scope);
        }

        throw new InvalidOperationException("Por aquí no vas a ningún sitio tampoco");
    }

    private (AssignmentNode, Scope scope) AnalyzeAssignmentExpression(Scope scope, AssignmentExpression assignmentExpression)
    {
        var right = AnalyzeExpression(assignmentExpression.Right, scope);
        var left = scope.Get(assignmentExpression.Left.Identifier);
        return left.Match(
            symbol => (new AssignmentNode(new KnownSymbolNode(symbol), right.Item1), scope),
            () => (new AssignmentNode(new UnknownSymbol(assignmentExpression.Left.Identifier), right.Item1)
            {
                Errors = [$"Use of undeclared variable '{assignmentExpression.Left.Identifier}'"]
            }, scope));
    }

    private (StatementNode, Scope) AnalyzeDeclaration(DeclarationStatement declarationStatement, Scope scope)
    {
        var declaration = scope
            .TryDeclare(new Symbol(declarationStatement.Name, IntType.Instance))
            .Match(
                s => (new DeclarationNode(declarationStatement.Name, s), s),
                _ => (new DeclarationNode(declarationStatement.Name, scope)
                {
                    Errors = [$"Variable {declarationStatement.Name} is already declared"]
                }, scope));

        return declaration;
    }
}

public class SymbolExpressionNode : ExpressionNode
{
    public SymbolNode SymbolNode { get; }

    public SymbolExpressionNode(SymbolNode symbolNode)
    {
        SymbolNode = symbolNode;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitSymbolExpression(this);
    }

    public override IEnumerable<SemanticNode> Children => [SymbolNode];
}

public class AddExpressionNode : BinaryExpressionNode
{
    public AddExpressionNode(ExpressionNode left, ExpressionNode right) : base(left, right, "+")
    {
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitAddition(this);
    }
}