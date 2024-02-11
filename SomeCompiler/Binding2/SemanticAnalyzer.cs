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

    private (FunctionNode, Scope) AnalyzeFunction(Function function, Scope scope)
    {
        return (new FunctionNode(function.Name, AnalyzeBlock(function.Block, scope)), scope);
    }

    private BlockNode AnalyzeBlock(Block block, Scope scope)
    {
        var statements = new List<StatementNode>();
        foreach (var statement in block)
        {
            var (analyzedStatement, newScope) = AnalyzeStatement(statement, scope);
            statements.Add(analyzedStatement);
            scope = newScope;
        }
        return new BlockNode(statements, scope);
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

        throw new InvalidOperationException("Por aquí no vas a ningún sitio");
    }

    private (AssignmentNode, Scope scope) AnalyzeAssignmentExpression(Scope scope, AssignmentExpression assignmentExpression)
    {
        var right = AnalyzeExpression(assignmentExpression.Right, scope);
        var left = scope.Get(assignmentExpression.Left.Identifier);
        return left.Match(
            symbol => (new AssignmentNode(symbol, right.Item1), scope),
            () => (new AssignmentNode(Symbol.Unknown(assignmentExpression.Left.Identifier), right.Item1)
            {
                Errors = [$"Use of undeclared variable {assignmentExpression.Left.Identifier}"]
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