namespace SomeCompiler.Parser.Tests;

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

    [Fact]
    public void Declaration()
    {
        var source = """
                     int main() 
                     { 
                        int a = 1;
                        int b = 2;                         
                     }
                     """;
        AssertParse(source);
    }

    [Fact]
    public void Multiple_lines()
    {
        var source = @"int main() { int b = 13; int a = 1; }";
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
    public void Arithmetic_addition()
    {
        var source = @"int main() { a = b + c; }";
        AssertParse(source);
    }
    
    [Fact]
    public void Arithmetic_mult()
    {
        var source = @"int main() { a = b * c; }";
        AssertParse(source);
    }
    
    [Fact]
    public void Equality()
    {
        var source = @"int main() { a = b == c; }";
        AssertParse(source);
    }
    
    [Fact]
    public void Inequality()
    {
        var source = @"int main() { a = b != c; }";
        AssertParse(source);
    }
    
    [Fact]
    public void Greater_than()
    {
        var source = @"int main() { a = b > c; }";
        AssertParse(source);
    }
    
    [Fact]
    public void Less_than()
    {
        var source = @"int main() { a = b < c; }";
        AssertParse(source);
    }
    
    [Fact]
    public void Less_than_or_equal()
    {
        var source = @"int main() { a = b <= c; }";
        AssertParse(source);
    }
    
    [Fact]
    public void Greater_than_or_equal()
    {
        var source = @"int main() { a = b >= c; }";
        AssertParse(source);
    }
    
    [Fact]
    public void True()
    {
        var source = @"int main() { a = true; }";
        AssertParse(source);
    }
    
    [Fact]
    public void False()
    {
        var source = @"int main() { a = false; }";
        AssertParse(source);
    }

    [Fact]
    public void Empty_return()
    {
        var source = @"int main() { return; }";
        AssertParse(source);
    }

    [Fact]
    public void If_statement_without_else()
    {
        var source = @"int main() { if (a > b) { return a; }}";
        AssertParse(source);
    }
    
    [Fact]
    public void If_statement_with_else()
    {
        var source = @"int main() { if (a > b) { return a; } else { return b; } }";
        AssertParse(source);
    }
    
    [Fact]
    public void Call()
    {
        var source = @"int main() { Func(13); }";
        AssertParse(source);
    }
    
    private static void AssertParse(string source)
    {
        var sut = new SomeParser();
        var result = sut.Parse(source);

        var visitor = new PrintNodeVisitor();
        result.Should().Succeed()
            .And.Subject.Value.ToSyntaxString().Should().BeEquivalentToIgnoringWhitespace(source);
    }
}