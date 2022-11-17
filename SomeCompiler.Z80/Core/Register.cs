namespace SomeCompiler.Z80.Core;

public class Register
{
    public string Name { get; }

    public Register(string name)
    {
        Name = name;
    }

    public static Register Hl => new("hl");
    public static Register A => new("a");
    public static Register L => new("l");
    public static Register B => new("b");

    public override string ToString()
    {
        return Name;
    }
}