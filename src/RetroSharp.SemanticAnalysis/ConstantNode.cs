namespace RetroSharp.SemanticAnalysis;

public class ConstantNode : ExpressionNode
{
    public object Value { get; }

    public ConstantNode(object value)
    {
        Value = value;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitConstant(this);
    }

    public override IEnumerable<SemanticNode> Children => [];
}