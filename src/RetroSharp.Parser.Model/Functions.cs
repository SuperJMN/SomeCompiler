using Zafiro.Core.Mixins;

namespace RetroSharp.Parser.Model;

public class Functions : List<Function>, INode
{
    public Functions(IEnumerable<Function> functions) : base(functions)
    {
    }

    public Functions()
    {
    }

    public IEnumerable<INode> Children => this;

    public override string ToString()
    {
        return this.JoinWithLines();
    }
}