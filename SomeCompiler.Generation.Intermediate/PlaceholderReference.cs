namespace SomeCompiler.Generation.Intermediate;

public class PlaceholderReference : Reference
{
    public override T Accept<T>(ICodeVisitor<T> toStringVisitor) => toStringVisitor.VisitPlaceholderReference(this);
}