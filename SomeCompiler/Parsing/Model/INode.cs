namespace SomeCompiler.Parsing.Model;

public interface INode
{
    public IEnumerable<INode> Children { get; }
}