namespace SomeCompiler.Parsing.Model;

public record DeclarationStatement(ArgumentType ArgumentType, string Identifier) : Statement
{
    public override string ToString()
    {
        return $"{ArgumentType} {Identifier};";
    }
}