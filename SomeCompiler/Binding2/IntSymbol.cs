using CSharpFunctionalExtensions;

namespace SomeCompiler.Binding2;

public record IntSymbol() : SymbolType("int")
{
    public static readonly IntSymbol Instance = new();
    public override string ToString() => base.ToString();
}