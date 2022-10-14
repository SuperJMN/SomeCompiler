namespace SomeCompiler.Parsing.Model;

internal record AssignmentExpression(LeftValue Left, Expression Right) : Expression
{
    public override IEnumerable<Expression> Children => new []{ Right };
}