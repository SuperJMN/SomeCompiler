using FluentAssertions;
using SomeCompiler.Parser;
using SomeCompiler.SemanticAnalysis;

namespace SomeCompiler.Generation.Intermediate.Tests;

public class IntermediateCodeGenerationTests
{
    [Fact(Skip="Temporarily skipped until testing approach is stabilized")]
    public void Test()
    {
        var input = "void main() { int a; int b; a = b + 1; }";

        var parser = new SomeParser();
        var semanticAnalyzer = new SemanticAnalyzer();
        var result = semanticAnalyzer.Analyze(parser.Parse(input).Value);
        var gen = new IntermediateCodeGenerator();
        var intermediate = gen.GenerateFunction((ProgramNode) result.Node);
        var stringVisitor = new ToStringVisitor();
        var str = intermediate.Value.Accept(stringVisitor);
        str.Should().BeEquivalentTo(
            """
            Function main:
            AssignReference: T1 = b
            AssignConstant: T2 = 1
            BinaryExpression: T3 = T1 + T2
            AssignReference: a = T3
            """);
    }
}