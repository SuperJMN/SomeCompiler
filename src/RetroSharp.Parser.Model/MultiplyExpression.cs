namespace RetroSharp.Parser.Model;

public record MultiplyExpression(Expression Left, Expression Right) : BinaryExpression(Left, Right)
{
    public override IEnumerable<INode> Children { get; }
    public override string Symbol => "*";
    public override int Precedence => 3;
    public override string ToString()
    {
        return base.ToString();
    }
}