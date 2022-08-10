using SomeCompiler.Compilation.Model;

namespace SomeCompiler.Intermediate;

public class IntermediateCodeProgram : List<IntermediateCode>
{
    public IntermediateCodeProgram(IEnumerable<IntermediateCode> instructions) : base(instructions)
    {
    }

    public override string ToString() => this.Join(";");
}