namespace SomeCompiler.Compilation.Model;

internal class BoundConstantExpression : BoundExpression
{
    public object Value { get; }

    public BoundConstantExpression(object value)
    {
        Value = value;
    }

    public override string? ToString() => Value.ToString();
}