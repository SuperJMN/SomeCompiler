using SomeCompiler.Utils;

namespace SomeCompiler.Binding;

public class BinderScope
{
    private readonly IScope<string, Symbol> inner;

    public BinderScope()
    {
        inner = new Scope<string, Symbol>(Maybe<IScope<string, Symbol>>.None);
    }

    public BinderScope(IScope<string, Symbol> inner)
    {
        this.inner = inner;
    }

    public Maybe<BinderScope> Parent => inner.Parent.Map(s => new BinderScope(s));

    public Result Declare(string key, Symbol value)
    {
        return inner.Declare(key, value);
    }

    public BinderScope CreateChild()
    {
        return new BinderScope(inner.CreateChild());
    }

    public Maybe<Symbol> Get(string key)
    {
        return inner.Get(key);
    }
}