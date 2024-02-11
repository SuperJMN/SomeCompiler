namespace SomeCompiler.Binding2;

public abstract class SemanticNode
{
    public abstract void Accept(INodeVisitor visitor);
    public IEnumerable<string> Errors { get; init; } = new List<string>();
}