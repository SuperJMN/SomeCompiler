using CSharpFunctionalExtensions;

namespace RetroSharp.Parser.Model;

public record IfElseStatement(Expression Condition, Block Then, Maybe<Block> Else) : Statement
{
    public override IEnumerable<INode> Children
    {
        get
        {
            yield return Condition;
            yield return Then;
            if (Else.HasValue)
            {
                yield return Else.Value;
            }
        }
    }
    public override string ToString()
    {
        var elseString = Else.HasValue ? $"\nelse\n{Else.Value}" : "";
        return $"if ({Condition})\n{Then}{elseString}";
    }
}