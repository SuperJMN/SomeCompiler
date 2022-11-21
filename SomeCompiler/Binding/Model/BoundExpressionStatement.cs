namespace SomeCompiler.Binding.Model;

internal record BoundExpressionStatement(BoundExpression Expression) : BoundStatement
{
    public override string ToString()
    {
        return Expression + ";";
    }
}