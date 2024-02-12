namespace SomeCompiler.Binding2;

public class FunctionNode : SemanticNode
{
    public string Name { get; }
    public BlockNode Block { get; }

    public FunctionNode(string name, BlockNode block)
    {
        Name = name;
        Block = block;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitFunctionNode(this);
    }

    public override IEnumerable<SemanticNode> Children => [Block];

    public override string ToString()
    {
        return $"void {Name}() {Block}";
    }
}