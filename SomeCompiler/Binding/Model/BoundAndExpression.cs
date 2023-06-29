namespace SomeCompiler.Binding.Model;

internal record BoundAndExpression(BoundExpression Left, BoundExpression Right) : BoundBinaryExpression(Left, Right)
{
    public override string Symbol => "&&";
    public override int Precedence => 11;
    public override string ToString()
    {
        return base.ToString();
    }
}