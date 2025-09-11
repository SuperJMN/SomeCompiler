namespace SomeCompiler.Parser.Model;

public abstract record Expression : INode
{
    public abstract IEnumerable<INode> Children { get; }
}