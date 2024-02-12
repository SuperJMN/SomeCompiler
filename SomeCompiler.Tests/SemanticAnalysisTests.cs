﻿using SomeCompiler.Binding2;

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

    [Fact]
    public void Assignment()
    {
        var input = "void main(){ int a; a = 1; }";
        var result = Analyze(input);

        result.Should().BeEquivalentToIgnoringWhitespace(input);
    }

    [Fact]
    public void Using_undeclared_variable_fails()
    {
        var input = "void main(){ a = 1; }";
        Errors(input).Should().ContainMatch("*undeclared*");
    }

    private IEnumerable<string> Errors(string input)
    {
        // Create a new instance of the SomeParser class.
        var parser = new SomeCompiler.Parser.Antlr4.SomeParser();
        // Parse the input string.
        var parseResult = parser.Parse(input);
        // Check if the parsing was successful.
        if (parseResult.IsFailure)
        {
            // If the parsing failed, return the error messages.
            return ["Can't analyze"];
        }

        var analyzer = new SemanticAnalyzer();
        var analyzeResult = analyzer.Analyze(parseResult.Value);
        return analyzeResult.GetAllErrors();
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
        var printNodeVisitor = new PrintNodeVisitor();
        analyzeResult.Accept(printNodeVisitor);
        return printNodeVisitor.ToString();
    }
}