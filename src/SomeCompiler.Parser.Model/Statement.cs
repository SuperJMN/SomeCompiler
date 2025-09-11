namespace SomeCompiler.Parser.Model;

public abstract record Statement : INode
{
    public abstract IEnumerable<INode> Children { get; }
}