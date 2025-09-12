using Zafiro.Core.Mixins;

namespace RetroSharp.SemanticAnalysis;

public class BlockNode : SemanticNode
{
    public BlockNode(IEnumerable<StatementNode> statements)
    {
        Statements = statements;
    }

    public IEnumerable<StatementNode> Statements { get; }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitBlockNode(this);
    }

    public override IEnumerable<SemanticNode> Children => Statements;

    public override string ToString()
    {
        var statements = Statements.Select(x => "\t" + x + ";").JoinWithLines();
        return $"\n{{\n{statements}\n}}";
    }

    public string ToString(int indentLevel)
    {
        var statements = Statements.Select(x => Enumerable.Repeat('\t', indentLevel + 1).AsString() + x + ";").JoinWithLines();
        return $"\n{{\n{statements}\n}}";
    }
}