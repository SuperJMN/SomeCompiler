namespace SomeCompiler.Parsing.Model;

public class Argument
{
    public Argument(ArgumentType argumentType, string identifier)
    {
        ArgumentType = argumentType;
        Identifier = identifier;
    }

    public ArgumentType ArgumentType { get; }
    public string Identifier { get; }

    public override string ToString() => $"{ArgumentType} {Identifier}";
}