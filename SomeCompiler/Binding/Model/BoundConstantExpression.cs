namespace SomeCompiler.Compilation.Model;

public record BoundConstantExpression(object Value) : BoundExpression
{
    public override string? ToString() => Value.ToString();
}