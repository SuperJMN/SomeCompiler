namespace RetroSharp.Core;

public class Operator : IHasPrecedence
{
    public string Symbol { get; }
    public int Precedence { get; }

    public static readonly Operator Multiplication = new Operator("*", 2);
    public static readonly Operator Division = new Operator("/", 2);
    public static readonly Operator Addition = new Operator("+", 3);
    public static readonly Operator Subtraction = new Operator("-", 3);
    public static readonly Operator LessThan = new Operator("<", 4);
    public static readonly Operator LessThanOrEqual = new Operator("<=", 4);
    public static readonly Operator GreatherThan = new Operator(">", 4);
    public static readonly Operator GreaterThanOrEqual = new Operator(">=", 4);
    public static readonly Operator Equal = new Operator("==", 5);
    public static readonly Operator NotEqual = new Operator("!=", 5);
    public static readonly Operator Not = new Operator("!", 6);
    public static readonly Operator Or = new Operator("||", 7);

    private Operator(string symbol, int precedence)
    {
        Symbol = symbol;
        Precedence = precedence;
    }

    public static Operator Get(string symbol)
    {
        return symbol switch
        {
            "+" => Addition,
            "-" => Subtraction,
            "*" => Multiplication,
            "/" => Division,
            "||" => Or,
            "!" => Not,
            "==" => Equal,
            "!=" => NotEqual,
            ">" => GreatherThan,
            "<" => LessThan,
            "<=" => LessThanOrEqual,
            ">=" => GreaterThanOrEqual,
            _ => throw new ArgumentException($"Invalid operator symbol: {symbol}")
        };
    }

}