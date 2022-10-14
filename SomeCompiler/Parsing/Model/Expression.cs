namespace SomeCompiler.Parsing.Model;

public abstract record Expression : INode
{
    public abstract IEnumerable<INode> Children { get; }
}