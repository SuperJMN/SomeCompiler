namespace SomeCompiler.Binding2;

public abstract class SemanticNode
{
    public abstract void Accept(INodeVisitor visitor);
}