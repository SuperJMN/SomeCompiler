namespace RetroSharp.Parser.Model;

public record IdentifierExpression(string Identifier) : Expression
{
    public override IEnumerable<Expression> Children => Enumerable.Empty<Expression>();

    public override string ToString()
    {
        return $"{Identifier}";
    }
}