using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using SomeCompiler.Compilation;
using SomeCompiler.Compilation.Model;

namespace SomeCompiler.Intermediate;

public class IntermediateCodeGenerator
{
    public Result<IntermediateCodeProgram, List<Error>> Generate(CompiledProgram compiledProgram)
    {
        IEnumerable<IntermediateCode> instructions = new []
        {
            Call("Main"),
            Halt(),
            Label("Main"),
            Return(),
        };
        
        return Result.Success<IntermediateCodeProgram, List<Error>>(new IntermediateCodeProgram(instructions));
    }

    private IntermediateCode Return()
    {
        return new ReturnCode();
    }

    private IntermediateCode Label(string name)
    {
        return new LabelCode(name);
    }

    private IntermediateCode Halt()
    {
        return new HaltCode();
    }

    private IntermediateCode Call(string label)
    {
        return new CallCode(label);
    }
}