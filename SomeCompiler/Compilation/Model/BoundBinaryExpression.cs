namespace SomeCompiler.Compilation.Model;

public record BoundBinaryExpression(BoundExpression Left, BoundExpression Right, BinaryOperator Operator) : BoundExpression;