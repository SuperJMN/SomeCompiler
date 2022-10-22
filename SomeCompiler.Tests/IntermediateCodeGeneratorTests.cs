using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using Xunit;

namespace SomeCompiler.Tests;

public class IntermediateCodeGeneratorTests
{
    [Fact]
    public void Empty()
    {
        AssertCode("void main() { }", "call main;halt;label main;return");
    }
    
    [Fact]
    public void Return_integer_constant()
    {
        AssertCode("void main() { return 1; }", "call main;halt;label main;T1=1;return T1");
    }

    private static void AssertCode(string input, string output)
    {
        var result = new CompilerFrontend().Generate(input);
        result
            .Should().BeSuccess()
            .And.Subject.Value.ToString().RemoveWhitespace()
            .Should().BeEquivalentTo(output.RemoveWhitespace());
    }
}