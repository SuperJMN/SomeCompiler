namespace SomeCompiler.Intermediate.Model;

public record Operator(string Symbol)
{
    public static Operator Add = new("+");
    public static Operator Multiply = new("*");
    public static Operator Subtract = new("-");
    public static Operator Divide = new("/");
    public static Operator Equal = new("=");

    public override string ToString()
    {
        return Symbol;
    }
}