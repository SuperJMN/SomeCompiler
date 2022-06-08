namespace SomeCompiler.Parsing.Model;

public class Block
{
    public Block(Statements statements)
    {
        Statements = statements;
    }

    public Statements Statements { get; }

    public override string ToString() => $"{{\n{Statements}\n}}";
}