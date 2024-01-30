using SomeCompiler.Binding2;
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

    [Fact]
    public void Given_scope_with_declaration_get_succeeds()
    {
        var scope = new Scope(Maybe<Scope>.None, Maybe<Dictionary<string, SymbolType>>.None);
        scope.TryDeclare("variable", IntType.Instance);
        scope.Get("variable").Should().HaveValue(IntType.Instance);
    }
    
    [Fact]
    public void Looking_for_undeclared_gives_error()
    {
        var scope = new Scope(Maybe<Scope>.None, Maybe<Dictionary<string, SymbolType>>.None);
        scope.Get("variable").Should().HaveNoValue();
    }
    
    [Fact]
    public void Declaring_same_symbol_gives_error()
    {
        var scope = Scope.Empty;
        scope.TryDeclare("variable", IntType.Instance).Should().Succeed();
        scope.TryDeclare("variable", IntType.Instance).Should().Fail();
    }
    
    [Fact]
    public void Given_child_scope_parent_symbols_are_reachable()
    {
        var scope = Scope.Empty;
        scope.TryDeclare("variable", IntType.Instance);
        var child = new Scope(scope);
        child.Get("variable").Should().HaveValue(IntType.Instance);
    }
}