namespace SomeCompiler.Parser;

public class AssignmentSyntax : StatementSyntax
{
    public AssignmentSyntax(LValue left, ExpressionSyntax right)
    {
        Left = left;
        Right = right;
    }

    public LValue Left { get; }
    public ExpressionSyntax Right { get; }
    
    public override void Accept(ISyntaxVisitor visitor)
    {
        visitor.VisitAssignment(this);
    }
}