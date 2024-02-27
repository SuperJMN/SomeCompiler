namespace SomeCompiler.SemanticAnalysis;

public abstract class BinaryExpressionNode : ExpressionNode
{
    public ExpressionNode Left { get; }
    public ExpressionNode Right { get; }
    public string Symbol { get; }

    public BinaryExpressionNode(ExpressionNode left, ExpressionNode right, string symbol)
    {
        Left = left;
        Right = right;
        Symbol = symbol;
    }

    public override IEnumerable<SemanticNode> Children => [Left, Right];
}