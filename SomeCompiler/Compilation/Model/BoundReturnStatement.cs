﻿namespace SomeCompiler.Compilation.Model;

internal class BoundReturnStatement : BoundStatement
{
    public BoundExpression? Expression { get; }

    public BoundReturnStatement(BoundExpression? expression)
    {
        Expression = expression;
    }

    public override string ToString() => $"return {Expression};";
}