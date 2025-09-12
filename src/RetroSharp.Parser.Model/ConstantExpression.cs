namespace RetroSharp.Parser.Model;

public record ConstantExpression(int Value) : Expression
{
    public override IEnumerable<Expression> Children => Enumerable.Empty<Expression>();

    public override string ToString()
    {
        return Value.ToString();
    }
}