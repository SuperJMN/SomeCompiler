namespace SomeCompiler.Binding2;

public class SymbolExpressionNode : ExpressionNode
{
    public SymbolExpressionNode(SymbolNode symbolNode)
    {
        SymbolNode = symbolNode;
    }

    public SymbolNode SymbolNode { get; }

    public override IEnumerable<SemanticNode> Children => [SymbolNode];

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitSymbolExpression(this);
    }
}