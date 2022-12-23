using Zafiro.Core.Mixins;

namespace SomeCompiler.Parser.Model;

public class ParameterList : List<Parameter>
{
    public ParameterList(IEnumerable<Parameter> arguments) : base(arguments)
    {
    }

    public ParameterList()
    {
    }

    public override string ToString()
    {
        return this.JoinWithCommas();
    }
}