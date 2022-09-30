using CSharpFunctionalExtensions;
using SomeCompiler.Compilation;
using SomeCompiler.Compilation.Model;
using SomeCompiler.Generation.Intermediate;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Parsing;

namespace SomeCompiler;

public class CompilerFrontend
{
    private readonly SomeParser parser;
    private readonly Compiler compiler;
    private readonly IntermediateCodeGenerator generator;

    public CompilerFrontend()
    {
        parser = new SomeParser();
        compiler = new Compiler();
        generator = new IntermediateCodeGenerator();
    }

    private Result<CompiledProgram, List<Error>> Compile(string source)
    {
        var parseResult = parser.Parse(source);

        var mapError = parseResult
            .MapError(x => x.Select(s => new Error(ErrorKind.SyntaxError, s)).ToList())
            .Bind(x => compiler.Compile(x));
        
        return mapError;
    }

    public Result<IntermediateCodeProgram, List<Error>> Emit(string source)
    {
        var compile = Compile(source);
        return compile.Bind(x => generator.Generate(x));
    }
}