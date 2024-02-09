using CSharpFunctionalExtensions;

namespace SomeCompiler.Binding2;

public record IntType() : SymbolType("int")
{
    public static readonly IntType Instance = new();
    public override string ToString() => base.ToString();
}