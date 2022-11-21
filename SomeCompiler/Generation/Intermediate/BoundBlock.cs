using SomeCompiler.Binding.Model;
using Zafiro.Core.Mixins;

namespace SomeCompiler.Generation.Intermediate;

public record BoundBlock(IEnumerable<BoundStatement> Statements) : BoundStatement
{
    public override string ToString()
    {
        return "{" + Environment.NewLine + Statements.JoinWithLines() + Environment.NewLine + "}";
    }
}