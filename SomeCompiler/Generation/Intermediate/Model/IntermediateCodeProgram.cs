using SomeCompiler.Generation.Intermediate.Model.Codes;
using Zafiro.Core.Mixins;

namespace SomeCompiler.Generation.Intermediate.Model;

public class IntermediateCodeProgram : List<Code>
{
    public IntermediateCodeProgram(IEnumerable<Code> codes) : base(codes)
    {
    }

    public override string ToString()
    {
        return this.ToTextFormatContent().Join(";");
    }
}