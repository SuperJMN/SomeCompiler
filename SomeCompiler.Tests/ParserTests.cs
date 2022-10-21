using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using SomeCompiler.Parsing;
using Xunit;

namespace SomeCompiler.Tests;

public class ParserTests
{
    [Fact]
    public void Empty_main()
    {
        var source = @"void main() { }";
        AssertParse(source);
    }

    [Theory]
    [InlineData(42)]
    [InlineData(1)]
    [InlineData(2304)]
    [InlineData(-1)]
    public void Return_integer_constant(int constant)
    {
        var source = $@"void main() {{ return {constant}; }}";
        AssertParse(source);
    }

    [Fact]
    public void Assignment()
    {
        var source = @"void main() { a = 12; }";
        AssertParse(source);
    }

    [Fact]
    public void Declaration()
    {
        var source = @"void main() { int a; }";
        AssertParse(source);
    }

    [Fact]
    public void Multiple_lines()
    {
        var source = @"void main() { b = 13; a = 1; }";
        AssertParse(source);
    }

    [Fact]
    public void More_than_one_function()
    {
        var source = @"void main() { } void another() { }";
        AssertParse(source);
    }

    [Fact]
    public void Function_with_arguments()
    {
        var source = @"void main(int a, int b) { }";
        AssertParse(source);
    }

    [Fact]
    public void Empty_return()
    {
        var source = @"void main() { return; }";
        AssertParse(source);
    }

    private static void AssertParse(string source)
    {
        var sut = new SomeParser();
        var result = sut.Parse(source);

        result.Should().BeSuccess()
            .And.Subject.Value.ToString().RemoveWhitespace().Should().Be(source.RemoveWhitespace());
    }
}