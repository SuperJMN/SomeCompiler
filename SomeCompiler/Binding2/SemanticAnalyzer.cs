using SomeCompiler.Parser.Model;

namespace SomeCompiler.Binding2;

public class SemanticAnalyzer
{
    public (SemanticNode, Scope) AnalyzeProgram(Program node)
    {
        var scope = Scope.Empty;
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
        return (new DeclarationNode(), scope.TryDeclare(declarationStatement.Name, IntType.Instance).Value);
    }

    public ProgramNode Analize(string input)
    {
        throw new NotImplementedException();
    }
}


internal class DeclarationNode : StatementNode
{}

public class StatementNode : SemanticNode
{}

public class BlockNode : SemanticNode
{
    public BlockNode(IEnumerable<StatementNode> statements, Scope scope)
    {
        Statements = statements;
        Scope = scope;
    }

    public IEnumerable<StatementNode> Statements { get; }
    public Scope Scope { get; }
}

public class FunctionNode : SemanticNode
{
    public string Name { get; }
    public BlockNode Block { get; }

    public FunctionNode(string name, BlockNode block)
    {
        Name = name;
        Block = block;
    }
}

public class ProgramNode : SemanticNode
{
    public List<FunctionNode> Functions { get; }

    public ProgramNode(List<FunctionNode> functions)
    {
        Functions = functions;
    }
}