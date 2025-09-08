using SomeCompiler.Generation.Intermediate.Model.Codes;
using Zafiro.Core.Mixins;

namespace SomeCompiler.Generation.Intermediate.Model;

public class IntermediateCodeProgram : List<SomeCompiler.Generation.Intermediate.Model.Codes.Code>
{
    public IntermediateCodeProgram()
    {
    }

    public IntermediateCodeProgram(IEnumerable<SomeCompiler.Generation.Intermediate.Model.Codes.Code> codes) : base(codes)
    {
    }

    public override string ToString()
    {
        return this.ToTextFormatContent().Join(";");
    }
}