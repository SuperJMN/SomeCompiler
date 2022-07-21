using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
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
        Compile("void main() { }");
    }
    
    [Fact]
    public void Duplicate_declaration()
    {
        var result = Compile("void main() { } void main() { }");
        result.Should()
            .BeFailure()
            .And
            .Subject.Error
            .Should()
            .Contain(e => e.Kind == ErrorKind.FunctionAlreadyDeclared);
    }
    
    [Fact]
    public void Main_function_should_be_declared()
    {
        var result = Compile("void other() { }");
        result.Should()
            .BeFailure()
            .And
            .Subject.Error
            .Should()
            .Contain(e => e.Kind == ErrorKind.MainNotDeclared);
    }
    
    private static Result<CompiledProgram, List<Error>> Compile(string source)
    {
        var parser = new SomeParser();
        var sut = new Compiler();

        var parseResult = parser.Parse(source);

        Assert.True(parseResult.IsSuccess, parseResult.ErrorMessage);
        return sut.Compile(parseResult.Result);
    }
}