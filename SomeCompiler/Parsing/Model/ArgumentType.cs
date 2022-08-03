namespace SomeCompiler.Parsing.Model;

public class ArgumentType
{
    public ArgumentType(string type)
    {
        Type = type;
    }

    public string Type { get; }

    public override string ToString() => Type;
}