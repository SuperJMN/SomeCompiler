namespace SomeCompiler.Binding.Model;

internal record BoundAddExpression(BoundExpression Left, BoundExpression Right) : BoundBinaryExpression(Left, Right)
{
    public override string Symbol => "+";
    public override int Precedence => 4;
    public override string ToString()
    {
        return base.ToString();
    }
}