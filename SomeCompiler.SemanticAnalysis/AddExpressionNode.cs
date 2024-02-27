namespace SomeCompiler.SemanticAnalysis;

public class AddExpressionNode : BinaryExpressionNode
{
    public AddExpressionNode(ExpressionNode left, ExpressionNode right) : base(left, right, "+")
    {
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitAddition(this);
    }
}