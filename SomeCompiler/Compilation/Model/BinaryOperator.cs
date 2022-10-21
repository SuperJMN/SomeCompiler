namespace SomeCompiler.Compilation.Model;

public class BinaryOperator
{
    public static BinaryOperator Add = new("+");
    public static BinaryOperator Multiply = new("*");
    public static BinaryOperator Divide = new("/");
    public static BinaryOperator Subtract = new("-");

    private BinaryOperator(string symbol)
    {
        Symbol = symbol;
    }

    public string Symbol { get; }

    public override string ToString()
    {
        return Symbol;
    }
}