namespace SomeCompiler.Binding2;

public class UnknownSymbol : SymbolNode
{
    public string Name { get; }

    public UnknownSymbol(string name)
    {
        Name = name;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitUnknownSymbol(this);
    }

    public override string ToString() => Name;

    public override IEnumerable<SemanticNode> Children => [];
}