namespace SomeCompiler.Parsing.Model;

public class CompoundStatement : List<Statement>, INode
{
    public CompoundStatement(IEnumerable<Statement> compoundStatement) : base(compoundStatement)
    {
    }

    public CompoundStatement()
    {
    }

    public IEnumerable<INode> Children => this.Select(x => x);
}