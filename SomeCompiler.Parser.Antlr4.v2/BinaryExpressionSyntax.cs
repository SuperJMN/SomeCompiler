namespace SomeCompiler.Parser;

public class BinaryExpressionSyntax : ExpressionSyntax
{
    public ExpressionSyntax Left { get; }
    public ExpressionSyntax Right { get; }
    public string Operator { get; }

    public BinaryExpressionSyntax(ExpressionSyntax left, ExpressionSyntax right, string @operator)
    {
        Left = left;
        Right = right;
        Operator = @operator;
    }
    
    public override void Accept(ISyntaxVisitor visitor)
    {
        visitor.VisitBinaryOperator(this);
    }
}