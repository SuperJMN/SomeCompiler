namespace SomeCompiler.Generation.Intermediate;

public class AssignReference : Code
{
    public Reference Target { get; }
    public Reference Source { get; }

    public AssignReference(Reference target, Reference source)
    {
        Target = target;
        Source = source;
    }

    public override T Accept<T>(ICodeVisitor<T> visitor) => visitor.VisitAssignReference(this);
}