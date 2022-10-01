namespace SomeCompiler.Parsing.Model;

public record Argument(ArgumentType ArgumentType, string Identifier)
{
    public override string ToString()
    {
        return $"{ArgumentType} {Identifier}";
    }
}