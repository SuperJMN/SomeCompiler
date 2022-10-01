namespace SomeCompiler.Parsing.Model;

public record CompoundStatement(Statements Statements) : Statement
{
    public override string ToString()
    {
        return $"{{{Environment.NewLine}{Statements.JoinWithLines()}{Environment.NewLine}}}";
    }
}