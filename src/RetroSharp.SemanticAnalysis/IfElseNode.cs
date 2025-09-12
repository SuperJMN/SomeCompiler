namespace RetroSharp.SemanticAnalysis;

public class IfElseNode : StatementNode
{
    public ExpressionNode Condition { get; }
    public BlockNode Then { get; }
    public Maybe<BlockNode> Else { get; }

    public IfElseNode(ExpressionNode condition, BlockNode thenBranch, Maybe<BlockNode> elseBranch)
    {
        Condition = condition;
        Then = thenBranch;
        Else = elseBranch;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitIfElse(this);
    }

    public override IEnumerable<SemanticNode> Children => Else.Match(
        e => new SemanticNode[] { Condition, Then, e },
        () => new SemanticNode[] { Condition, Then }
    );
}

