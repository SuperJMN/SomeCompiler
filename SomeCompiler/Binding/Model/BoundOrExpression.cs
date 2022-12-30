namespace SomeCompiler.Binding.Model;

internal record BoundOrExpression(BoundExpression Left, BoundExpression Right) : BoundBinaryExpression(Left, Right)
{
    public override string Symbol => "||";
    public override int Precedence => 12;
    public override string ToString()
    {
        return base.ToString();
    }
}