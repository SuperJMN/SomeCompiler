using CSharpFunctionalExtensions.ValueTasks;
using SomeCompiler.Parser;

namespace SomeCompiler;

//public class Compiler
//{
//    private readonly SomeParser parser;
//    private readonly SemanticAnalyzer analizer;
//    private readonly IntermediateCodeGenerator generator;

//    public Compiler()
//    {
//        parser = new SomeParser();
//        analizer = new SemanticAnalyzer();
//        generator = new IntermediateCodeGenerator();
//    }

//    private Result<CompiledProgram, List<string>> Compile(string source)
//    {
//        var parseResult = parser.Parse(source);

//        var mapError = parseResult
//            .MapError(x => x.Select(s => new Error(ErrorKind.SyntaxError, s)).ToList())
//            .Bind(x => analizer.Analyze(x));
        
//        return mapError;
//    }

//    public Result<IntermediateCodeProgram, List<string>> Emit(string source)
//    {
//        var compile = Compile(source);
//        return compile.Bind(x => generator.Generate(x));
//    }
//}