namespace SomeCompiler.Parsing.Model;

public record AssignmentExpression(LeftValue LeftValue, Expression Expression) : Expression
{
    public override string ToString()
    {
        return $"{LeftValue} = {Expression};";
    }
}