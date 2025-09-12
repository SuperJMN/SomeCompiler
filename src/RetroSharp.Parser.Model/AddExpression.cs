namespace RetroSharp.Parser.Model;

public record AddExpression(Expression Left, Expression Right) : BinaryExpression(Left, Right)
{
    public override IEnumerable<INode> Children { get; }
    public override string Symbol => "+";
    public override int Precedence => 4;
    public override string ToString()
    {
        return base.ToString();
    }
}