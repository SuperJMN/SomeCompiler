namespace SomeCompiler.Parsing.Model;

internal class DeclarationStatement : Statement
{
    public DeclarationStatement(ArgumentType argumentType, string identifier)
    {
        ArgumentType = argumentType;
        Identifier = identifier;
    }

    public ArgumentType ArgumentType { get; }
    public string Identifier { get; }

    public override string ToString() => $"{ArgumentType} {Identifier};";
}