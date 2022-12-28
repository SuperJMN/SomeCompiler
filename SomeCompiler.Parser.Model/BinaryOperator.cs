namespace SomeCompiler.Parser.Model;

public class BinaryOperator : IComparable<BinaryOperator>
{
    public static BinaryOperator Add = new("+", 5);
    public static BinaryOperator Subtract = new("-", 5);
    public static BinaryOperator Multiply = new("*", 6);
    public static BinaryOperator Divide = new("/", 6);
    
    private BinaryOperator(string symbol, int precedence)
    {
        Symbol = symbol;
        Precedence = precedence;
    }

    public string Symbol { get; }

    public override string ToString()
    {
        return Symbol;
    }

    public int Precedence { get; }

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

    public static bool operator >(BinaryOperator a, BinaryOperator b)
    {
        return a.Precedence > b.Precedence;
    }

    public static bool operator <(BinaryOperator a, BinaryOperator b)
    {
        return a.Precedence < b.Precedence;
    }
}