﻿namespace SomeCompiler.Binding.Model;

public record BoundReturnStatement(Maybe<BoundExpression> Expression) : BoundStatement
{
    public override string ToString() => $"return {Expression};";
}