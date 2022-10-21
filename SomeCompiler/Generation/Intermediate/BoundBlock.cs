using SomeCompiler.Compilation.Model;

namespace SomeCompiler.Generation.Intermediate;

public record BoundBlock(IEnumerable<BoundStatement> Statements) : BoundStatement
{
    public override string ToString()
    {
        return "{" + Environment.NewLine + Statements.JoinWithLines() + Environment.NewLine + "}";
    }
}