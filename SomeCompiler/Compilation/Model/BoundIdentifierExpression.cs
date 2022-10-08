namespace SomeCompiler.Compilation.Model;

public record class BoundIdentifierExpression : BoundExpression
{
    public BoundIdentifierExpression(string identifier)
    {
        Identifier = identifier;
    }

    public string Identifier { get; }
}