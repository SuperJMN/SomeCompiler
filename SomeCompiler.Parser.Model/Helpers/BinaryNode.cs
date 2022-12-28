namespace SomeCompiler.Parser.Model.Helpers;

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