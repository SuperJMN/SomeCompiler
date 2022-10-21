namespace SomeCompiler.Parsing.Model;

public record LeftValue(string Identifier)
{
    public override string ToString()
    {
        return Identifier;
    }
}