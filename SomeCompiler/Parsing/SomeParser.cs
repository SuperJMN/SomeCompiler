using CSharpFunctionalExtensions;
using SomeCompiler.Parsing.Model;

namespace SomeCompiler.Parsing;

public class SomeParser
{
    public Result<Program, List<string>> Parse(string source)
    {
        var arithmeticGrammar = new SomeGrammar();
        var parser = arithmeticGrammar.BuildCompiler<Program>();
        var compilationResult = parser.Compile(source);
        return compilationResult.IsSuccess
            ? Result.Success<Program, List<string>>(compilationResult.Result)
            : Result.Failure<Program, List<string>>(new List<string> { compilationResult.ErrorMessage });
    }
}