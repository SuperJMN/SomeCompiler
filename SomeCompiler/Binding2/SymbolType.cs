namespace SomeCompiler.Binding2;

public abstract record SymbolType(string Name)
{
    public override string ToString() => Name;
}