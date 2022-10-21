namespace SomeCompiler.Parsing.Model;

public abstract record Expression : INode
{
    public abstract IEnumerable<INode> Children { get; }

    public override string ToString()
    {
        return "Expression not supported";
    }
}