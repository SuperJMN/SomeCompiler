namespace SomeCompiler.Parsing.Model;

public record NegateExpression(Expression Expression) : Expression
{
    public override string ToString()
    {
        return $"-{Expression}";
    }
}