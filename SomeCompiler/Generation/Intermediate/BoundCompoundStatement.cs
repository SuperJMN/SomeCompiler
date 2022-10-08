using SomeCompiler.Compilation.Model;

namespace SomeCompiler.Generation.Intermediate;

public record BoundCompoundStatement(IEnumerable<BoundStatement> Statements) : BoundStatement
{
    public override string ToString()
    {
        return "{" + Environment.NewLine + Statements.JoinWithLines() + Environment.NewLine + "}";
    }
}