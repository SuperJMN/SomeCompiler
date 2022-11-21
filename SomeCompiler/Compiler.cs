using SomeCompiler.Binding;
using SomeCompiler.Binding.Model;
using SomeCompiler.Generation.Intermediate;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Parsing;

namespace SomeCompiler;

public class Compiler
{
    private readonly SomeParser parser;
    private readonly Binder binder;
    private readonly IntermediateCodeGenerator generator;

    public Compiler()
    {
        parser = new SomeParser();
        binder = new Binder();
        generator = new IntermediateCodeGenerator();
    }

    private Result<CompiledProgram, List<Error>> Compile(string source)
    {
        var parseResult = parser.Parse(source);

        var mapError = parseResult
            .MapError(x => x.Select(s => new Error(ErrorKind.SyntaxError, s)).ToList())
            .Bind(x => binder.Compile(x));
        
        return mapError;
    }

    public Result<IntermediateCodeProgram, List<Error>> Emit(string source)
    {
        var compile = Compile(source);
        return compile.Bind(x => generator.Generate(x));
    }
}