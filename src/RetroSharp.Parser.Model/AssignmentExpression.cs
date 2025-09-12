namespace RetroSharp.Parser.Model;

public record AssignmentExpression(LeftValue Left, Expression Right) : Expression
{
    public override IEnumerable<Expression> Children => new[] { Right };

    public override string ToString()
    {
        return $"{Left} = {Right}";
    }
}