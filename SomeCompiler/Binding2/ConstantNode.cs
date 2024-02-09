namespace SomeCompiler.Binding2;

public class ConstantNode : ExpressionNode
{
    public int Value { get; }

    public ConstantNode(int value)
    {
        Value = value;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitConstant(this);
    }
}