namespace SomeCompiler.Parsing.Model;

internal record ConstantExpression(int Value) : Expression
{
    public override IEnumerable<Expression> Children => Enumerable.Empty<Expression>();
}