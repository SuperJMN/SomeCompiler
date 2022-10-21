namespace SomeCompiler.Parsing.Model;

public record Argument(ArgumentType ArgumentType, string Name)
{
    public override string ToString()
    {
        return $"{ArgumentType} {Name}";
    }
}