using Antlr4.Runtime.Tree;
using MoreLinq;

namespace SomeCompiler.Parser.Antlr4;

public static class ParseNodeExtensions
{
    public static IEnumerable<IParseTree> Children(this IParseTree node)
    {
        return Enumerable.Range(0, node.ChildCount).Select(node.GetChild);
    }

    public static T? Descendant<T>(this IParseTree node)
    {
        return node.Descendants<T>().FirstOrDefault();
    }

    public static IEnumerable<T> Descendants<T>(this IParseTree node)
    {
        return MoreEnumerable
            .TraverseBreadthFirst(node, x => x.Children())
            .Flatten()
            .OfType<T>();
    }
}