namespace RetroSharp.Generation.Intermediate.Model;

public record Operator(string Symbol)
{
    public static Operator Add = new("+");
    public static Operator Multiply = new("*");
    public static Operator Subtract = new("-");
    public static Operator Divide = new("/");
    public static Operator Equal = new("=");
    public static Operator Call = new("Call");
    public static Operator Halt = new("Halt");
    public static Operator Label = new("Label");
    public static Operator Return = new("Return");

    public override string ToString()
    {
        return Symbol;
    }
}