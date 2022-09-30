namespace SomeCompiler.Compilation.Model;

public class BoundBinaryExpression : BoundExpression
{
    public BoundBinaryExpression(BoundExpression left, BoundExpression right, BinaryOperator @operator)
    {
        Left = left;
        Right = right;
        Operator = @operator;
    }

    public BoundExpression Left { get; }
    public BoundExpression Right { get; }
    public BinaryOperator Operator { get; }
}