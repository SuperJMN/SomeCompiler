namespace SomeCompiler.Parsing.Model;

public class Statements : List<Statement>
{
    public Statements(IEnumerable<Statement> statements) : base(statements)
    {
    }

    public Statements(params Statement[] statements) : base(statements)
    {
    }

    public override string ToString()
    {
        return this.JoinWithLines();
    }
}