
using RetroSharp.Core;

namespace RetroSharp.SemanticAnalysis;

public class BinaryExpressionNode : ExpressionNode
{
    public ExpressionNode Left { get; }
    public ExpressionNode Right { get; }
    public Operator Operator { get; }

    public BinaryExpressionNode(ExpressionNode left, ExpressionNode right, Operator op)
    {
        Left = left;
        Right = right;
        Operator = op;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitBinaryExpression(this);
    }

    public override IEnumerable<SemanticNode> Children => [Left, Right];
}