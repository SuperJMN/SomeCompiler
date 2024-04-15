namespace SomeCompiler.Generation.Intermediate;

public abstract class Code
{
    public abstract T Accept<T>(ICodeVisitor<T> visitor);
}