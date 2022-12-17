namespace SomeCompiler.Binding.Model;

public class BinaryOperator : IComparable<BinaryOperator>
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

    public int Precedence { get; set; }

    public int CompareTo(BinaryOperator? other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        if (Precedence < other.Precedence)
        {
            return -1;
        }

        if (Precedence > other.Precedence)
        {
            return 1;
        }

        return 0;
    }
}