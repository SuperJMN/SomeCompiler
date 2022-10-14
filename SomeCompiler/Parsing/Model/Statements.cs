namespace SomeCompiler.Parsing.Model;

public class Statements : List<Statement>, INode
{
    public Statements(Statements statements) : base(statements)
    {
    }

    public Statements()
    {
    }

    public IEnumerable<INode> Children => this;
}