namespace SomeCompiler.Parser;

public record Error<T>(T Symbol, int Line, int Column, string Message);