namespace RetroSharp.Parser.Model;

public record UnaryExpression(string Operator, Expression Expression) : Expression
{
    public override IEnumerable<INode> Children => new[] { Expression };

    public override string ToString()
    {
        return $"{Operator}{Expression}";
    }
}