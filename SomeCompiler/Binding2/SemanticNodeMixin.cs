using MoreLinq;

namespace SomeCompiler.Binding2;

public static class SemanticNodeMixin
{
    public static IList<string> GetAllErrors(this SemanticNode root)
    {
        return MoreEnumerable.TraverseBreadthFirst(root, node => node.Children).SelectMany(x => x.Errors).ToList();
    }
}