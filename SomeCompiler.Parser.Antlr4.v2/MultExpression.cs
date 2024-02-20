namespace SomeCompiler.Parser;

public class MultExpression : ExpressionSyntax
{
    public MultExpression(ExpressionSyntax left, ExpressionSyntax right)
    {
        Left = left;
        Right = right;
    }

    public override void Accept(ISyntaxVisitor visitor)
    {
        visitor.VisitMult(this);
    }

    public ExpressionSyntax Left { get; }
    public ExpressionSyntax Right { get; }
}