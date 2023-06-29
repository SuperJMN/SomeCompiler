using SomeCompiler.Parser.Model;

namespace SomeCompiler.Tests;

public class ScopeTests
{
    [Fact]
    public void ScopeTest()
    {
        var sut = new BinderScope();
        var symbol = new Symbol(BoundType.Int);
        sut.Declare("pepito", symbol);
        var result = sut.Get("pepito");
        result.HasValue.Should().BeTrue();
        result.Value.Should().Be(symbol);
    }

    [Fact]
    public void ScopeTest2()
    {
        var sut = new BinderScope();
        var symbol = new Symbol(BoundType.Int);
        sut.Declare("pepito", symbol);
        var scope = sut.CreateChild();
        scope.Declare("flokito", new Symbol(BoundType.Int));

        var result = scope.Get("pepito");

        result.HasValue.Should().BeTrue();
        result.Value.Should().Be(symbol);
    }
}