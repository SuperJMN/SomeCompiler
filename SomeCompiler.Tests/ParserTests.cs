using Antlr4.Runtime;
using SomeCompiler.Parser.Antlr4;

namespace SomeCompiler.Tests;

public class ParserTests
{
    [Fact]
    public void Empty_main()
    {
        var source = @"int main() { }";
        AssertParse(source);
    }

    [Theory]
    [InlineData(42)]
    [InlineData(1)]
    [InlineData(2304)]
    [InlineData(-1)]
    public void Return_integer_constant(int constant)
    {
        var source = $@"int main() {{ return {constant}; }}";
        AssertParse(source);
    }

    [Fact]
    public void Assignment()
    {
        var source = @"int main() { a = 12; }";
        AssertParse(source);
    }

    [Fact(Skip = "Grammar doesn't support it yet")]
    public void Declaration()
    {
        var source = @"int main() { int a; }";
        AssertParse(source);
    }

    [Fact]
    public void Multiple_lines()
    {
        var source = @"int main() { b = 13; a = 1; }";
        AssertParse(source);
    }

    [Fact]
    public void More_than_one_function()
    {
        var source = @"int main() { } int another() { }";
        AssertParse(source);
    }

    [Fact]
    public void Function_with_arguments()
    {
        var source = @"int main(int a, int b) { }";
        AssertParse(source);
    }

    [Fact]
    public void Arithmetic()
    {
        var source = @"int main() { a = b + -c; }";
        AssertParse(source);
    }

    [Fact]
    public void Empty_return()
    {
        var source = @"int main() { return; }";
        AssertParse(source);
    }

    private static void AssertParse(string source)
    {
        var sut = new Parser.Antlr4.Parser();
        var result = sut.Parse(source);

        result.Should().BeSuccess()
            .And.Subject.Value.ToString().RemoveWhitespace().Should().Be(source.RemoveWhitespace());
    }
}

public class ExpressionParsingTests
{
    [Theory]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("identifier")]
    [InlineData("1+1")]
    [InlineData("1-1")]
    [InlineData("1*2")]
    [InlineData("!a")]
    [InlineData("a=1")]
    [InlineData("id=1")]
    public void Expression(string input)
    {
        AssertExpression(input);
    }

    private void AssertExpression(string s)
    {
        var lexer = new CLexer(CharStreams.fromString(s));
        var parser = new CParser(new CommonTokenStream(lexer));
        var expr = parser.expression();

        var ret = new ExpressionConverter().ParseExpression(expr);
        ret.ToString().RemoveWhitespace().Should().Be(s.RemoveWhitespace());
    }
}