using Zafiro.Core.Mixins;

namespace SomeCompiler.Parser.Model;

public class Block : List<Statement>, INode
{
    public Block(IEnumerable<Statement> compoundStatement) : base(compoundStatement)
    {
    }

    public Block()
    {
    }

    public IEnumerable<INode> Children => this.Select(x => x);

    public override string ToString()
    {
        return "{" + this.JoinWithLines() + "}";
    }
}