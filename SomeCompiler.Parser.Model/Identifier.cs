namespace SomeCompiler.Parser.Model;

public record Identifier : INode
{
    public string Name { get; }

    public Identifier(string name)
    {
        Name = name;
    }

    public IEnumerable<INode> Children => new List<INode>();
}