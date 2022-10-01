namespace SomeCompiler.Parsing.Model;

public record ConstantExpression(int Constant) : Expression
{
    public override string ToString()
    {
        return Constant.ToString();
    }
}