namespace SomeCompiler.Intermediate.Model;

public static class CodeMixin
{
    public static IEnumerable<string> ToCodeFormatContent(this IEnumerable<Code> codes)
    {
        return CodeFormatter.ToCodeFormatContent(codes);
    }
}