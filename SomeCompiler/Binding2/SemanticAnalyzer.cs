using SomeCompiler.Parser.Model;
using Zafiro.Core.Mixins;

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

public class DeclarationNode : StatementNode
{
    public string Name { get; }
    public Scope Scope { get; }

    public DeclarationNode(string name, Scope scope)
    {
        Name = name;
        Scope = scope;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitDeclarationNode(this);
    }

    public override string ToString() => Scope.Get(Name).Value + " " + Name;
}

public abstract class StatementNode : SemanticNode;

public class BlockNode : SemanticNode
{
    public BlockNode(IEnumerable<StatementNode> statements, Scope scope)
    {
        Statements = statements;
        Scope = scope;
    }

    public IEnumerable<StatementNode> Statements { get; }
    public Scope Scope { get; }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitBlockNode(this);
    }

    public override string ToString()
    {
        var statements = Statements.Select(x => "\t" + x + ";").JoinWithLines();
        return $"\n{{\n{statements}\n}}";
    }
    
    public string ToString(int indentLevel)
    {
        var statements = Statements.Select(x => Enumerable.Repeat('\t', indentLevel+1).AsString() + x + ";").JoinWithLines();
        return $"\n{{\n{statements}\n}}";
    }
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

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitFunctionNode(this);
    }

    public override string ToString()
    {
        return $"void {Name}() {Block}";
    }
}

public class ProgramNode : SemanticNode
{
    public List<FunctionNode> Functions { get; }

    public ProgramNode(List<FunctionNode> functions)
    {
        Functions = functions;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitProgramNode(this);
    }

    public override string ToString()
    {
        return Functions.JoinWithLines();
    }
}

public interface INodeVisitor
{
    void VisitDeclarationNode(DeclarationNode node);
    void VisitBlockNode(BlockNode node);
    void VisitFunctionNode(FunctionNode node);
    void VisitProgramNode(ProgramNode node);
}