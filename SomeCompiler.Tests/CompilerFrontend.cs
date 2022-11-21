using CSharpFunctionalExtensions;
using SomeCompiler.Binding;
using SomeCompiler.Binding.Model;
using SomeCompiler.Generation.Intermediate;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Parsing;

namespace SomeCompiler.Tests;

public class CompilerFrontend
{
    private readonly SomeParser parser;
    private readonly Binder binder;
    private readonly IntermediateCodeGenerator generator;

    public CompilerFrontend()
    {
        parser = new SomeParser();
        binder = new Binder();
        generator = new IntermediateCodeGenerator();
    }

    public Result<CompiledProgram, List<Error>> Compile(string source)
    {
        var parseResult = parser.Parse(source);

        var mapError = parseResult
            .MapError(x => x.Select(s => new Error(ErrorKind.SyntaxError, s)).ToList())
            .Bind(x => binder.Compile(x));
        
        return mapError;
    }

    public Result<IntermediateCodeProgram, List<Error>> Generate(string source)
    {
        var compile = Compile(source);
        return compile.Bind(program => generator.Generate(program));
    }
}