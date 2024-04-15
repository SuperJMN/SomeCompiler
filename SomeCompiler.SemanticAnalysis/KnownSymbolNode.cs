namespace SomeCompiler.SemanticAnalysis;

public class KnownSymbolNode : SymbolNode
{
    public Symbol Symbol { get; }

    public KnownSymbolNode(Symbol symbol)
    {
        Symbol = symbol;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitKnownSymbol(this);
    }

    public override IEnumerable<SemanticNode> Children { get; } = [];
}