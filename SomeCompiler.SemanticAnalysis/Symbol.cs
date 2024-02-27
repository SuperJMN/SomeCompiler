namespace SomeCompiler.SemanticAnalysis;

public record Symbol(string Name, SymbolType Type)
{
    public override string ToString() => Type + " " + Name;
}