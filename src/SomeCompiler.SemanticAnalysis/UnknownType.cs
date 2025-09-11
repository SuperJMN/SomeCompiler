namespace SomeCompiler.SemanticAnalysis;

public record UnknownType(string Name) : SymbolType(Name)
{
    public static SymbolType Instance { get; } = new UnknownType("__UNKNOWN__");
}