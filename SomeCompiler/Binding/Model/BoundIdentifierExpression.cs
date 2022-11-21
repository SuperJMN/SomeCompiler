namespace SomeCompiler.Binding.Model;

public record BoundIdentifierExpression(string Identifier) : BoundExpression
{
    public override string ToString()
    {
        return Identifier;
    }
}