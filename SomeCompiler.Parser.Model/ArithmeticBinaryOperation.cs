namespace SomeCompiler.Parser.Model;

public record ArithmeticBinaryOperation(BinaryOperator Op, params Expression[] Expressions) : Expression
{
    public override IEnumerable<Expression> Children => Expressions;

    public override string ToString()
    {
        if (Expressions.Length == 2)
        {
           return $"{Format(Expressions[0])} {Op} {Format(Expressions[1])}";
        }

        if (Expressions.Length == 1)
        {
            return Op.ToString() + Expressions[0];
        }

        return "Not supported";
    }

    private string Format(Expression expression)
    {
        if (expression is ArithmeticBinaryOperation aop && Op > aop.Op)
        {
            return "(" + expression + ")";
        }

        return expression.ToString();
    }
}