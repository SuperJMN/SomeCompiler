namespace SomeCompiler.Parsing.Model;

public record ArithmeticOperation(string? Op, params Expression[] Expressions) : Expression
{
    public override IEnumerable<Expression> Children => Expressions;

    public override string ToString()
    {
        if (Expressions.Length == 2)
        {
            return $"{Expressions[0]} {Op} {Expressions[1]}";
        }

        if (Expressions.Length == 1)
        {
            return (Op ?? "") + Expressions[0];
        }

        return "Not supported";
    }
}