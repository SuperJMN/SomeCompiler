namespace SomeCompiler.Binding2;

public class AssignmentNode : ExpressionNode
{
    public SymbolNode Left { get; }
    public ExpressionNode Right { get; }

    public AssignmentNode(SymbolNode left, ExpressionNode right)
    {
        Left = left;
        Right = right;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitAssignment(this);
    }

    public override IEnumerable<SemanticNode> Children => [Left, Right];
}