namespace SomeCompiler.Parser.Model;

public record IdentifierExpression(string Identifier) : Expression
{
    public override IEnumerable<Expression> Children => Enumerable.Empty<Expression>();
}