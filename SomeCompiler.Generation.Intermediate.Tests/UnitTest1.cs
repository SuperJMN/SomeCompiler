using SomeCompiler.Core;
using SomeCompiler.Parser;
using SomeCompiler.SemanticAnalysis;

namespace SomeCompiler.Generation.Intermediate.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var input = "void main() { int a; int b; a = b + 1; }";

        var parser = new SomeParser();
        var semanticAnalyzer = new SemanticAnalyzer();
        var result = semanticAnalyzer.Analyze(parser.Parse(input).Value);
        var gen = new IntermediateCodeGenerator();
        var codes = gen.GenerateFunction((ProgramNode) result.Node);
    }
}