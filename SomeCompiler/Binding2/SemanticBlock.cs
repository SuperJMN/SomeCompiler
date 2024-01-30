namespace SomeCompiler.Binding2;

public class SemanticBlock : SemanticNode
{
    public SemanticBlock(List<SemanticNode> statements, Scope scope)
    {
        Statements = statements;
        Scope = scope;
    }

    public List<SemanticNode> Statements { get; }
    public Scope Scope { get; }
}