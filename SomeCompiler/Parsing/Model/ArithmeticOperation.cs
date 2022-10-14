namespace SomeCompiler.Parsing.Model;

public record ArithmeticOperation(string? Op, params Expression[] Expressions) : Expression
{
    public override IEnumerable<Expression> Children => Expressions;
}