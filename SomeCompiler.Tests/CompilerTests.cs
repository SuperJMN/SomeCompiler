using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using Microsoft.CSharp.RuntimeBinder;
using SomeCompiler.Compilation;
using SomeCompiler.Compilation.Model;
using SomeCompiler.Parsing;
using Xunit;

namespace SomeCompiler.Tests;

public class CompilerTests
{
    [Fact]
    public void Empty_program()
    {
        AssertSuccess("void main() { }");
    }

    [Fact]
    public void Return_numeric_constant()
    {
        AssertSuccess("void main() { return 13; }");
    }

    [Fact]
    public void Duplicate_declaration()
    {
        AssertError("void main() { } void main() { }", ErrorKind.FunctionAlreadyDeclared);
    }

    [Fact]
    public void Main_function_should_be_declared()
    {
        AssertError("void other() { }", ErrorKind.MainNotDeclared);
    }

    private static void AssertSuccess(string code)
    {
        Compile(code)
            .Should().BeSuccess()
            .And.Subject.Value.ToString().RemoveWhitespace()
            .Should().Be(code.RemoveWhitespace());
    }

    private static void AssertError(string input, ErrorKind error)
    {
        var result = Compile(input);
        result.Should()
            .BeFailure()
            .And
            .Subject.Error
            .Should()
            .Contain(e => e.Kind == error);
    }

    private static Result<CompiledProgram, List<Error>> Compile(string source)
    {
        return new CompilerFrontend().Compile(source);
    }
}