namespace RetroSharp.SemanticAnalysis;

public abstract record SymbolType(string Name)
{
    public override string ToString() => Name;
    public static readonly SymbolType Unknown = UnknownType.Instance;
}