using CSharpFunctionalExtensions;

namespace SomeCompiler.Parsing.Model;

record ReturnStatement(Maybe<Expression> Expression) : Statement
{
    public override IEnumerable<INode> Children => Expression.ToList();
}