namespace SomeCompiler.Parser.Model;

public record DeclarationStatement(ArgumentType ArgumentType, string Name) : Statement
{
    public override IEnumerable<INode> Children => new List<INode>();

    public override string ToString()
    {
        return $"{ArgumentType} {Name};";
    }
}