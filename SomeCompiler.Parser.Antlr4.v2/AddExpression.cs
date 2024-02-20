namespace SomeCompiler.Parser;

public class AddExpression : ExpressionSyntax
{
    public AddExpression(ExpressionSyntax left, ExpressionSyntax right)
    {
        Left = left;
        Right = right;
    }

    public override void Accept(ISyntaxVisitor visitor)
    {
        visitor.VisitAdd(this);
    }

    public ExpressionSyntax Left { get; set; }
    public ExpressionSyntax Right { get; set; }
}