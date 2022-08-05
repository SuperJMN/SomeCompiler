using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using Xunit;

namespace SomeCompiler.Tests;

public class IntermediateCodeGeneratorTests
{
    [Fact(Skip = "Next test to make green")]
    public void Empty()
    {
        var input = "void main() { }";
        var output = "Call Main;Halt;Label Main;Return;";

        var result = new CompilerFrontend().Generate(input);
        result
            .Should().BeSuccess()
            .And.Subject.Value.ToString().RemoveWhitespace()
            .Should().Be(output.RemoveWhitespace());
    }
}