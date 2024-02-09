using SomeCompiler.Utils;

namespace SomeCompiler.Binding;

public class BinderScope
{
    private readonly IScope<string, SymbolType> inner;

    public BinderScope()
    {
        inner = new Scope<string, SymbolType>(Maybe<IScope<string, SymbolType>>.None);
    }

    public BinderScope(IScope<string, SymbolType> inner)
    {
        this.inner = inner;
    }

    public Maybe<BinderScope> Parent => inner.Parent.Map(s => new BinderScope(s));

    public Result Declare(string key, SymbolType value)
    {
        return inner.Declare(key, value);
    }

    public BinderScope CreateChild()
    {
        return new BinderScope(inner.CreateChild());
    }

    public Maybe<SymbolType> Get(string key)
    {
        return inner.Get(key);
    }
}