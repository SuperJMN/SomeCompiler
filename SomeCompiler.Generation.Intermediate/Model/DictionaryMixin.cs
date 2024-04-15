namespace SomeCompiler.Generation.Intermediate.Model;

public static class DictionaryMixin
{
    public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> valueFactory)
    {
        if (dict.TryGetValue(key, out var value))
        {
            return value;
        }

        value = valueFactory();
        dict.Add(key, value);
        return value;
    }
}