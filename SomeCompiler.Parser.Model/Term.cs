namespace SomeCompiler.Parser.Model;

public record Term(string Op, params Expression[] Expressions) : ArithmeticOperation(Op, Expressions)
{
    public override string ToString()
    {
        return $"{base.ToString()}";
    }
}