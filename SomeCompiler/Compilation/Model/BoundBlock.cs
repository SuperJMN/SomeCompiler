namespace SomeCompiler.Compilation.Model;

public class BoundBlock : List<BoundStatement>
{
    public BoundBlock(IEnumerable<BoundStatement> statements) : base(statements)
    {
    }
    
    public override string ToString() => new object[] { "{", this.JoinWithLines(), "}" }.JoinWithLines();
}