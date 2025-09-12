namespace RetroSharp.Parser.Tests;

public static class SyntaxMixin
{
    public static string ToSyntaxString(this ISyntax syntax)
    {
        var visitor = new PrintNodeVisitor();
        syntax.Accept(visitor);
        return visitor.ToString();
    }
}