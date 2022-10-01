namespace SomeCompiler.Compilation.Model;

internal record BoundExpressionStatement(BoundExpression Expression) : BoundStatement
{
    public override string ToString()
    {
        return Expression + ";";
    }
}