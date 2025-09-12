using CSharpFunctionalExtensions;
using RetroSharp.Parser;
using RetroSharp.SemanticAnalysis;

namespace RetroSharp.SemanticAnalysis.Tests;

internal static class SemanticTestDriver
{
    public static Result<ProgramNode> Analyze(string source)
    {
        var parser = new SomeParser();
        var parse = parser.Parse(source);
        if (parse.IsFailure)
        {
            return Result.Failure<ProgramNode>(parse.Error);
        }

        var analyzer = new SemanticAnalyzer();
        var result = analyzer.Analyze(parse.Value);
        return Result.Success((ProgramNode)result.Node);
    }
}
