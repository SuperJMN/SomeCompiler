namespace SomeCompiler.Binding2;

public class AssignmentNode : ExpressionNode
{
    public Symbol Left { get; }
    public ExpressionNode Right { get; }

    public AssignmentNode(Symbol left, ExpressionNode right)
    {
        Left = left;
        Right = right;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitAssignment(this);
    }
}