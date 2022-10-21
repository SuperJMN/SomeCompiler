namespace SomeCompiler.Parsing.Model;

internal record ExpressionStatement(Expression Expression) : Statement
{
    public override IEnumerable<INode> Children => Expression.Children;

    public override string ToString()
    {
        return Expression + ";";
    }
}