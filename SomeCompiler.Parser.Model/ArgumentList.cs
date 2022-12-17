using Zafiro.Core.Mixins;

namespace SomeCompiler.Parser.Model;

public class ArgumentList : List<Argument>
{
    public ArgumentList(IEnumerable<Argument> arguments) : base(arguments)
    {
    }

    public ArgumentList()
    {
    }

    public override string ToString()
    {
        return this.JoinWithCommas();
    }
}