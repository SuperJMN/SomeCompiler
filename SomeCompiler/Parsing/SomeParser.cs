using CSharpFunctionalExtensions;
using EasyParse.Parsing;
using SomeCompiler.Parsing.Model;

namespace SomeCompiler.Parsing;

public class SomeParser
{
    public static Lazy<Compiler<Program>> Parser = new(GetParser, LazyThreadSafetyMode.ExecutionAndPublication);

    public Result<Program, List<string>> Parse(string source)
    {
        var parser = Parser.Value;
        var compilationResult = parser.Compile(source);
        return compilationResult.IsSuccess
            ? Result.Success<Program, List<string>>(compilationResult.Result)
            : Result.Failure<Program, List<string>>(new List<string> { compilationResult.ErrorMessage });
    }

    private static Compiler<Program> GetParser()
    {
        var arithmeticGrammar = new SomeGrammar();
        return arithmeticGrammar.BuildCompiler<Program>();
    }
}