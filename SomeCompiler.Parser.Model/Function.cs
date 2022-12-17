namespace SomeCompiler.Parser.Model;

public record Function(string Name, ArgumentList ArgumentList, Block Block) : INode
{
    public IEnumerable<INode> Children => Block.Children;

    public override string ToString()
    {
        return $"int {Name}({ArgumentList}) {Block}";
    }
}