namespace SomeCompiler.Z80.Core;

public class Register
{
    public string Name { get; }

    public Register(string name)
    {
        Name = name;
    }

    public static Register HL => new("HL");
    public static Register A => new("A");
    public static Register L => new("L");
    public static Register B => new("B");
    public static Register BC => new("BC");
    public static Register DE => new("DE");

    public override string ToString()
    {
        return Name;
    }
}