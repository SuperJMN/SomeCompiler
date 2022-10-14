namespace SomeCompiler.Parsing.Model;

public record ReturnType(string Type)
{
    public override string ToString()
    {
        return Type;
    }
}