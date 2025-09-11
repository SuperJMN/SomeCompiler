using CSharpFunctionalExtensions;

namespace SomeCompiler.Parser.Model;

public record ReturnStatement(Maybe<Expression> Expression) : Statement
{
    public override IEnumerable<INode> Children => Expression.ToList();

    public override string ToString()
    {
        return "return" + Expression.Match(expression => " " + expression, () => "") + ";";
    }
}