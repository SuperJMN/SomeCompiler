namespace SomeCompiler.Parser.Model;

public record DivideExpression(Expression Left, Expression Right) : BinaryExpression(Left, Right)
{
    public override IEnumerable<INode> Children { get; }
    public override string Symbol => "/";
    public override int Precedence => 3;
    public override string ToString()
    {
        return base.ToString();
    }
}