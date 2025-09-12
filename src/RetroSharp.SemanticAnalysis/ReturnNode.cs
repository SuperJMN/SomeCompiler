namespace RetroSharp.SemanticAnalysis;

public class ReturnNode : StatementNode
{
    public Maybe<ExpressionNode> Expression { get; }

    public ReturnNode(Maybe<ExpressionNode> expression)
    {
        Expression = expression;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitReturn(this);
    }

    public override IEnumerable<SemanticNode> Children => Expression.Match(
        e => new[] { e },
        () => Array.Empty<SemanticNode>()
    );
}

