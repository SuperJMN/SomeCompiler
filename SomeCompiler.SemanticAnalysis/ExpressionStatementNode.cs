namespace SomeCompiler.SemanticAnalysis;

public class ExpressionStatementNode : StatementNode
{
    public ExpressionNode Expression { get; }

    public ExpressionStatementNode(ExpressionNode expression)
    {
        Expression = expression;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitExpressionStatement(this);
    }

    public override IEnumerable<SemanticNode> Children => [Expression];
}