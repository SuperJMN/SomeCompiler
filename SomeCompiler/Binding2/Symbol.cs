namespace SomeCompiler.Binding2;

public record Symbol(string Name, SymbolType Type)
{
    public override string ToString() => Type + " " + Name;

    public static Symbol Unknown(string name)
    {
        return new Symbol(name, SymbolType.Unknown);
    }
}