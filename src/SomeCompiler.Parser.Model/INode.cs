namespace SomeCompiler.Parser.Model;

public interface INode
{
    public IEnumerable<INode> Children { get; }
}