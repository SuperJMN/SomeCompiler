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
    public void Division()
    {
        AssertSuccess("int main() { return 4/2; }");
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

    [Fact]
    public void Variable_declaration()
    {
        AssertSuccess("int main() { int p; }");
    }

    private static void AssertSuccess(string code)
    {
        Compile(code)
            .Should().Succeed()
            .And.Subject.Value.ToString().Should().BeEquivalentToIgnoringWhitespace(code);
    }

    private static void AssertError(string input, ErrorKind error)
    {
        var result = Compile(input);
        result.Should()
            .Fail()
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