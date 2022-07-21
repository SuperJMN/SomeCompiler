using System.Collections.ObjectModel;

namespace SomeCompiler.Compilation.Model;

public class BoundBlock : Collection<BoundStatement>
{
    public override string ToString() => new object[]{ "{", this.JoinWithLines(), "}" }.JoinWithLines();
}