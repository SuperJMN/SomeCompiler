namespace SomeCompiler.Parsing.Model;

public record Program(Functions Functions) : INode
{
    public IEnumerable<INode> Children => Functions.Children;
}