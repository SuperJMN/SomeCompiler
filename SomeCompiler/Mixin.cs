namespace SomeCompiler.Compilation.Model;

public static class Mixin
{
    public static string JoinWithLines<T>(this IEnumerable<T> items)
    {
        return Join(items, Environment.NewLine);
    }
    
    public static string JoinWithCommas<T>(this IEnumerable<T> items)
    {
        return Join(items, Environment.NewLine);
    }
    
    public static string Join<T>(this IEnumerable<T> items, string separator)
    {
        return string.Join(separator, items.Select(x => x.ToString()));
    }
}