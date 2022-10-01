using CSharpFunctionalExtensions;

namespace SomeCompiler.Parsing.Model;

public record ReturnStatement : Statement
{
    public ReturnStatement(Maybe<Expression> expression)
    {
        Expression = expression;
    }

    public Maybe<Expression> Expression { get; }

    public override string ToString()
    {
        return $"return {Expression.Match(x => x.ToString(), () => "")};";
    }
}