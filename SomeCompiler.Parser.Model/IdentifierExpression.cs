namespace SomeCompiler.Parser.Model;

internal record IdentifierExpression(string Identifier) : Expression
{
    public override IEnumerable<Expression> Children => Enumerable.Empty<Expression>();
}