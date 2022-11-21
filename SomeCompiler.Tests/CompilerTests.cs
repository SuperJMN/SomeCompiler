namespace SomeCompiler.Tests;

public class CompilerTests
{
    [Fact]
    public void Empty_program()
    {
        AssertSuccess("int main() { }");
    }

    [Fact]
    public void Return_numeric_constant()
    {
        AssertSuccess("int main() { return 13; }");
    }
    
    [Fact]
    public void Assignment()
    {
        AssertSuccess("int main() { a = 13; }");
    }

    [Fact]
    public void Addition()
    {
        AssertSuccess("int main() { return 1+2; }");
    }

    [Fact]
    public void Duplicate_declaration()
    {
        AssertError("int main() { } int main() { }", ErrorKind.FunctionAlreadyDeclared);
    }

    [Fact]
    public void Main_function_should_be_declared()
    {
        AssertError("int other() { }", ErrorKind.MainNotDeclared);
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