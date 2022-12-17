namespace SomeCompiler.Parser.Model;

public record ExpressionStatement(Expression Expression) : Statement
{
    public override IEnumerable<INode> Children => Expression.Children;

    public override string ToString()
    {
        return Expression + ";";
    }
}