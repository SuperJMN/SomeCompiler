using CSharpFunctionalExtensions;
using SomeCompiler.Compilation;
using SomeCompiler.Compilation.Model;

namespace SomeCompiler.Intermediate;

public class IntermediateCodeGenerator
{
    public Result<IntermediateCodeProgram, List<Error>> Generate(CompiledProgram result)
    {
        return Result.Failure<IntermediateCodeProgram, List<Error>>(new List<Error>());
    }
}

public class IntermediateCodeProgram : List<IntermediateCode>
{
}

public class IntermediateCode
{
}