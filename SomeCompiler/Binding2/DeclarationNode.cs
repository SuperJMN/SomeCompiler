namespace SomeCompiler.Binding2;

public class DeclarationNode : StatementNode
{
    public string Name { get; }
    public Scope Scope { get; }

    public DeclarationNode(string name, Scope scope)
    {
        Name = name;
        Scope = scope;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitDeclarationNode(this);
    }

    public override string ToString() => Scope.Get(Name).Value + " " + Name;
}