using SomeCompiler.Parsing.Model;

namespace SomeCompiler.Binding.Model;

public record BoundAssignmentExpression(LeftValue Left, BoundExpression Right) : BoundExpression
{
    public override string ToString()
    {
        return $"{Left} = {Right}";
    }
}