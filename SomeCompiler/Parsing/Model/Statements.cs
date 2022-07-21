using SomeCompiler.Compilation.Model;

namespace SomeCompiler.Parsing.Model;

public class Statements : List<Statement>
{
    public Statements(IEnumerable<Statement> items) : base(items.ToList())
    {
    }

    public Statements()
    {
    }

    public override string ToString() => this.JoinWithLines();
}