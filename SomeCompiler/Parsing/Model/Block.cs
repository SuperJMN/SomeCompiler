using SomeCompiler.Compilation.Model;

namespace SomeCompiler.Parsing.Model;

public class Block : List<Statement>
{
    public Block(Statements statements) : base(statements)
    {
    }

    public override string ToString() => new object[] { "{", this.JoinWithLines(), "}" }.JoinWithLines();

}