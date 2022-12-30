namespace SomeCompiler.Parser.Model;

public record AndExpression(Expression Left, Expression Right) : BinaryExpression(Left, Right)
{
    public override IEnumerable<INode> Children { get; }
    public override string Symbol => "&&";
    public override int Precedence => 11;
    public override string ToString()
    {
        return base.ToString();
    }
}