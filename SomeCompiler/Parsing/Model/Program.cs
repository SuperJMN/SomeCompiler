using Zafiro.Core.Mixins;

namespace SomeCompiler.Parsing.Model;

public record Program(Functions Functions) : INode
{
    public IEnumerable<INode> Children => Functions.Children;

    public override string ToString()
    {
        return Functions.JoinWithLines();
    }
}