using CSharpFunctionalExtensions;

namespace SomeCompiler.Parser.Model;

public record ReturnStatement : Statement
{
    public ReturnStatement(Maybe<Expression> Expression)
    {
        this.Expression = Expression;
    }

    public override IEnumerable<INode> Children => Expression.ToList();
    public Maybe<Expression> Expression { get; init; }

    public override string ToString()
    {
        return "return" + Expression.Match(expression => " " + expression, () => "") + ";";
    }

    public void Deconstruct(out Maybe<Expression> Expression)
    {
        Expression = this.Expression;
    }
}