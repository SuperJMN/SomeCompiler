namespace SomeCompiler.Binding.Model;

public record BoundBinaryExpression(BoundExpression Left, BoundExpression Right, BinaryOperator Operator) : BoundExpression
{
    public override string ToString()
    {
        return $"{Left} {Operator} {Right}";
    }
}