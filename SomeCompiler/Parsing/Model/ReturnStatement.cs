namespace SomeCompiler.Parsing.Model;

internal class ReturnStatement : Statement
{
    public ReturnStatement(Expression expression)
    {
        Expression = expression;
    }

    public ReturnStatement()
    {
    }

    public Expression? Expression { get; }

    public override string ToString()
    {
        return $"return {Expression};";
    }
}