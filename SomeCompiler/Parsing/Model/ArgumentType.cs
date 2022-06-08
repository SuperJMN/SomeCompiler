namespace SomeCompiler.Parsing.Model;

public class ArgumentType
{
    public string Type { get; }

    public ArgumentType(string type)
    {
        Type = type;
    }

    public override string ToString() => Type;
}