namespace SomeCompiler.Parser.Model;

public record Parameter(ArgumentType ArgumentType, string Name)
{
    public override string ToString()
    {
        return $"{ArgumentType} {Name}";
    }
}