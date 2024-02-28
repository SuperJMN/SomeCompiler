namespace SomeCompiler.Generation.Intermediate;

public class Halt : Code
{
    public override T Accept<T>(ICodeVisitor<T> visitor) => visitor.VisitHalt(this);
}