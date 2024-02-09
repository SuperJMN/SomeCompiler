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
                break;
            case IfElseStatement ifElseStatement:
                break;
            case ReturnStatement returnStatement:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(statement));
        }

        throw new InvalidOperationException();
    }

    private (StatementNode, Scope) AnalyzeDeclaration(DeclarationStatement declarationStatement, Scope scope)
    {
        var declScope = scope.TryDeclare(declarationStatement.Name, IntType.Instance).Value;
        return (new DeclarationNode(declarationStatement.Name, declScope), declScope);
    }
}