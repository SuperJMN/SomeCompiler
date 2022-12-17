namespace SomeCompiler.Parser.Model;

public record Argument(ArgumentType ArgumentType, string Name)
{
    public override string ToString()
    {
        return $"{ArgumentType} {Name}";
    }
}