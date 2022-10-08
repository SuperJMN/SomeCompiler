namespace SomeCompiler.Parsing.Model;

public record ExpressionStatement(Expression Expression) : Statement
{
    public override string ToString()
    {
        return Expression.ToString();
    }
}