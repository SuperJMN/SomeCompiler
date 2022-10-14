namespace SomeCompiler.Parsing.Model;

public record Term(string Op, params Expression[] Expressions) : ArithmeticOperation(Op, Expressions);