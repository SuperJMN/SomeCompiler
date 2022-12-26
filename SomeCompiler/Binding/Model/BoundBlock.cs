using Zafiro.Core.Mixins;

namespace SomeCompiler.Binding.Model;

public record BoundBlock(IEnumerable<BoundStatement> Statements) : BoundStatement
{
    public override string ToString()
    {
        return "{" + Environment.NewLine + Statements.JoinWithLines() + Environment.NewLine + "}";
    }
}