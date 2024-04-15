namespace SomeCompiler.Generation.Intermediate;

public class IntermediateCodeProgram : List<Code>
{
    public IntermediateCodeProgram(List<Code> value) : base(value)
    {
    }

    public T Accept<T>(ICodeVisitor<T> visitor)
    {
        return visitor.VisitProgram(this);
    }
}