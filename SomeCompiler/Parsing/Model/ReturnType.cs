namespace SomeCompiler.Parsing;

public class ReturnType
{
    public string Type { get; }

    public ReturnType(string type)
    {
        Type = type;
    }

    public override string ToString() => $"{Type}";
}