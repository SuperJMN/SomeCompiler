using CSharpFunctionalExtensions;
using SomeCompiler.Compilation.Model;
using SomeCompiler.Parsing.Model;

namespace SomeCompiler.Compilation;

public class Compiler
{
    private List<Error> errors = new();

    public Result<CompiledProgram, List<Error>> Compile(Program source)
    {
        return new Result<CompiledProgram, List<Error>>();
    }
}