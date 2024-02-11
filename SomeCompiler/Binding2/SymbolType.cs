namespace SomeCompiler.Binding2;

public abstract record SymbolType(string Name)
{
    public override string ToString() => Name;
    public static readonly SymbolType Unknown = UnknownType.Instance;
}

public record UnknownType(string Name) : SymbolType(Name)
{
    public static SymbolType Instance { get; } = new UnknownType("__UNKNOWN__");
}