namespace SomeCompiler.Parsing;

public class ReturnType
{
    public ReturnType(string type)
    {
        Type = type;
    }

    public string Type { get; }

    public override string ToString() => $"{Type}";
}