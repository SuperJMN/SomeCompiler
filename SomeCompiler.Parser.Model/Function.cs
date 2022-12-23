namespace SomeCompiler.Parser.Model;

public record Function(ReturnType ReturnType, string Name, ParameterList ParameterList, Block Block) : INode
{
    public IEnumerable<INode> Children => Block.Children;

    public override string ToString()
    {
        return $"{ReturnType} {Name}({ParameterList}) {Block}";
    }
}