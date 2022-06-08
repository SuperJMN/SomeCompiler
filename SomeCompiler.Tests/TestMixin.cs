namespace SomeCompiler.Tests;

public static class TestMixin
{
    public static string RemoveWhitespace(this string str)
    {
        var array = str.Where(c => !char.IsWhiteSpace(c)).ToArray();
        return new string(array);
    }
}