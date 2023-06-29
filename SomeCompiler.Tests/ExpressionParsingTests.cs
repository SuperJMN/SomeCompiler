using Antlr4.Runtime;
using SomeCompiler.Parser.Antlr4;

namespace SomeCompiler.Tests;

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
    [InlineData("7/2*4")]
    [InlineData("7-2+4")]
    [InlineData("7*2+4/2")]
    [InlineData("(1+2)*4")]
    [InlineData("1 && 0")]
    [InlineData("(1 && 2) + 2")]
    [InlineData("0 || 1")]
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