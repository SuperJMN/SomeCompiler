using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using Xunit;

namespace SomeCompiler.Tests;

public class IntermediateCodeGeneratorTests
{
    [Fact]
    public void Empty()
    {
        AssertCode("void main() { }", "Call Main;Halt;Label Main;Return");
    }
    
    [Fact]
    public void Return_integer_constant()
    {
        AssertCode("void main() { return 1; }", "Call Main;Halt;Label Main;Return 1");
    }

    private static void AssertCode(string input, string output)
    {
        var result = new CompilerFrontend().Generate(input);
        result
            .Should().BeSuccess()
            .And.Subject.Value.ToString().RemoveWhitespace()
            .Should().Be(output.RemoveWhitespace());
    }
}