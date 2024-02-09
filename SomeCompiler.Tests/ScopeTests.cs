using SomeCompiler.Binding2;

namespace SomeCompiler.Tests;

public class ScopeTests
{
    [Fact]
    public void ScopeTest()
    {
        var sut = new BinderScope();
        var symbol = new Binding.SymbolType(BoundType.Int);
        sut.Declare("pepito", symbol);
        var result = sut.Get("pepito");
        result.HasValue.Should().BeTrue();
        result.Value.Should().Be(symbol);
    }

    [Fact]
    public void ScopeTest2()
    {
        var sut = new BinderScope();
        var symbol = new Binding.SymbolType(BoundType.Int);
        sut.Declare("pepito", symbol);
        var scope = sut.CreateChild();
        scope.Declare("flokito", new Binding.SymbolType(BoundType.Int));

        var result = scope.Get("pepito");

        result.HasValue.Should().BeTrue();
        result.Value.Should().Be(symbol);
    }

    [Fact]
    public void Given_scope_with_declaration_get_succeeds()
    {
        var scope = new Scope(Maybe<Scope>.None, Maybe<Dictionary<string, Symbol>>.None);
        scope.TryDeclare(new Symbol("variable", IntType.Instance));
        scope.Get("variable").Should().HaveValue(new Symbol("variable", IntType.Instance));
    }
    
    [Fact]
    public void Looking_for_undeclared_gives_error()
    {
        var scope = Scope.Empty;
        scope.Get("variable").Should().HaveNoValue();
    }
    
    [Fact]
    public void Declaring_same_symbol_gives_error()
    {
        var scope = Scope.Empty;
        scope.TryDeclare(new Symbol("variable", IntType.Instance)).Should().Succeed();
        scope.TryDeclare(new Symbol("variable", IntType.Instance)).Should().Fail();
    }
    
    [Fact]
    public void Given_child_scope_parent_symbols_are_reachable()
    {
        var scope = Scope.Empty;
        scope.TryDeclare(new Symbol("variable", IntType.Instance));
        var child = new Scope(scope);
        child.Get("variable").Should().HaveValue(new Symbol("variable", IntType.Instance));
    }
}