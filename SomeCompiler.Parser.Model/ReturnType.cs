namespace SomeCompiler.Parser.Model;

public record ReturnType(string Type) : INode
{
    public override string ToString()
    {
        return Type;
    }

    public IEnumerable<INode> Children => new List<INode>();
}