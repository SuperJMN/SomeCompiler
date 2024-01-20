using SomeCompiler.Parser.Model;

namespace SomeCompiler.Parser.Antlr4;

public record IfStatement(Expression Condition, Block ThenStatement) : Statement
{
    public override IEnumerable<INode> Children
    {
        get
        {
            yield return Condition;
            yield return ThenStatement;
        }
    }

    public override string ToString()
    {
        return $"if ({Condition})\n{ThenStatement}";
    }
}