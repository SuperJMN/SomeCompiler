namespace SomeCompiler.Utils;

public class Scope<TKey, TValue> : IScope<TKey, TValue> where TKey : notnull
{
    public Maybe<IScope<TKey, TValue>> Parent { get; }
    private readonly Dictionary<TKey, TValue> declarations = new();

    public Scope() : this(Maybe<IScope<TKey, TValue>>.None)
    {
    }

    public Scope(Maybe<IScope<TKey, TValue>> parent)
    {
        Parent = parent;
    }

    public Result Declare(TKey key, TValue value)
    {
        return declarations.TryAdd(key, value) ? Result.Success() : Result.Failure($"{key} is already declared");
    }

    public Maybe<TValue> Get(TKey key)
    {
        return declarations
            .TryFind(key)
            .Or(() => Parent.Bind(scope => scope.Get(key)));
    }

    public IScope<TKey, TValue> CreateChild()
    {
        return new Scope<TKey, TValue>(this);
    }
}