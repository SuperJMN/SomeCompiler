using SomeCompiler.Parser.Model;

namespace SomeCompiler.Binding2;

public class SemanticAnalyzer
{
    private (SemanticNode, Scope) AnalyzeProgram(Program node)
    {
        var scope = Scope.Empty;
        var functions = new List<FunctionNode>();
        foreach (var statement in node.Functions)
        {
            var (functionNode, newScope) = AnalyzeFunction(statement, scope);
            scope = newScope;
            functions.Add(functionNode);
        }
        return (new SemanticProgramNode(functions), scope);
    }

    private (FunctionNode, Scope) AnalyzeFunction(Function function, Scope scope)
    {
        return (new FunctionNode(function.Name, AnalyzeBlock(function.Block, scope).Item1), scope);
    }
    private (BlockNode, Scope) AnalyzeBlock(Block block, Scope scope)
    {
        return (new BlockNode(block.Select(statement => AnalyzeStatement(statement, scope).Item1)), scope);
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
        throw new NotImplementedException();
    }
}


internal class DeclarationStatementNode : StatementNode
{
}

internal class StatementNode : SemanticNode
{
}

internal class BlockNode : SemanticNode
{
    public IEnumerable<StatementNode> Statements { get; }

    public BlockNode(IEnumerable<StatementNode> statements)
    {
        Statements = statements;
    }
}

internal class FunctionNode : SemanticNode
{
    public FunctionNode(string functionName, BlockNode block)
    {
        throw new NotImplementedException();
    }
}

internal class SemanticProgramNode : SemanticNode
{
    public SemanticProgramNode(List<FunctionNode> functions)
    {
        throw new NotImplementedException();
    }
}