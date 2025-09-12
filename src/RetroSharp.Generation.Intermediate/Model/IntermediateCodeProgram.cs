using RetroSharp.Generation.Intermediate.Model.Codes;
using Zafiro.Core.Mixins;

namespace RetroSharp.Generation.Intermediate.Model;

public class IntermediateCodeProgram : List<RetroSharp.Generation.Intermediate.Model.Codes.Code>
{
    public IntermediateCodeProgram()
    {
    }

    public IntermediateCodeProgram(IEnumerable<RetroSharp.Generation.Intermediate.Model.Codes.Code> codes) : base(codes)
    {
    }

    public override string ToString()
    {
        return this.ToTextFormatContent().Join(";");
    }
}