namespace SomeCompiler.Parser.Model.Helpers;

public static class BinaryTreeHelper
{
    public static BinaryNode<T>? Build<T>(IList<T> array, int skip = 0)
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
        var right = Build(array, skip + 2);
        return new BinaryNode<T>(list[1], left, right);
    }
}

public class BinaryNode<T>
{
    public BinaryNode<T>? Left { get; }
    public BinaryNode<T>? Right { get; }
    public T Value { get; }

    public BinaryNode(T value)
    {
        Value = value;
    }

    public BinaryNode(T value, BinaryNode<T>? left, BinaryNode<T>? right) : this(value)
    {
        Left = left;
        Right = right;
    }
}