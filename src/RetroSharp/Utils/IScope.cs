namespace RetroSharp.Utils;

public interface IScope<TKey, TValue> where TKey : notnull
{
    Maybe<IScope<TKey, TValue>> Parent { get; }
    Result Declare(TKey key, TValue value);
    IScope<TKey, TValue> CreateChild();
    Maybe<TValue> Get(TKey key);
}