namespace SomeCompiler.Parsing.Model;

public record Unit(string? Op, params Expression[] Expressions) : ArithmeticOperation(Op, Expressions);