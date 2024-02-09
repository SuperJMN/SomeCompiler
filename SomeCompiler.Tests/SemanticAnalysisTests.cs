using SomeCompiler.Binding2;

namespace SomeCompiler.Tests;

public class SemanticAnalysisTests
{
    [Fact]
    public void Empty_program()
    {
        var input = "";
        var result = Analyze(input);
        result.Should().BeEquivalentToIgnoringWhitespace(input);
    }
    
    [Fact]
    public void Empty_main()
    {
        var input = "void main(){}";
        var result = Analyze(input);
        result.Should().BeEquivalentToIgnoringWhitespace(input);
    }
    
    [Fact]
    public void Declaration()
    {
        var input = "void main(){ int a; }";
        var result = Analyze(input);

        result.Should().BeEquivalentToIgnoringWhitespace(input);
    }

    private static string Analyze(string input)
    {
        // Create a new instance of the SomeParser class.
        var parser = new SomeCompiler.Parser.Antlr4.SomeParser();
        // Parse the input string.
        var parseResult = parser.Parse(input);
        // Check if the parsing was successful.
        if (parseResult.IsFailure)
        {
            // If the parsing failed, return the error messages.
            return string.Join("\n", parseResult.Error);
        }
        // Create a new instance of the SemanticAnalyzer class.
        var analyzer = new SemanticAnalyzer();
        // Analyze the parsed program.
        var analyzeResult = analyzer.Analyze(parseResult.Value);
        // Convert the result to a string and return it.
        return analyzeResult.ToString();
    }

}