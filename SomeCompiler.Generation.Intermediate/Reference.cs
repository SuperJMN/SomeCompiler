namespace SomeCompiler.Generation.Intermediate;

public abstract class Reference
{
    public abstract T Accept<T>(ICodeVisitor<T> toStringVisitor);
}