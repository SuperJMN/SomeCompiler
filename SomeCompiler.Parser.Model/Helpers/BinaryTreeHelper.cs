namespace SomeCompiler.Parser.Model.Helpers;

public static class BinaryTreeHelper
{
    public static BinaryNode<T>? FromPostFix<T>(IList<T> array, int skip = 0)
    {
        var list = array.Skip(skip).Take(2).ToList();
	
        if (!list.Any())
        {
            return default;
        }
	
        if (list.Count == 1) 
        {
            return new BinaryNode<T>(list[0]);
        }

        var left = new BinaryNode<T>(list[0]);
        var right = FromPostFix(array, skip + 2);
        return new BinaryNode<T>(list[1], left, right);
    }
}