namespace SomeCompiler.Parsing.Model;

public record ArgumentType(string Type)
{
    public override string ToString()
    {
        return $"{Type}";
    }
}