namespace SomeCompiler.Intermediate.Model;

public record AssignmentExpression(IdentifierExpression Identifier, Expression Expression) : Expression;