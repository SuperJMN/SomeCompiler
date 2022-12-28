namespace SomeCompiler.Binding.Model;

internal record BoundMultiplyExpression(BoundExpression Left, BoundExpression Right) : BoundBinaryExpression(Left, Right)
{
    public override string Symbol => "*";
    public override int Precedence => 3;
    public override string ToString()
    {
        return base.ToString();
    }
}