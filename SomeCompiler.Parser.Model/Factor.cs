namespace SomeCompiler.Parser.Model;

public record Factor(string Op, params Expression[] Expressions) : ArithmeticOperation(Op, Expressions)
{
    public override string ToString()
    {
        return $"{base.ToString()}";
    }
}