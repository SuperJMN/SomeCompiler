namespace SomeCompiler.Binding2;

public class SemanticVarDecl : SemanticNode
{
    public SemanticVarDecl(string name, SymbolType type, Scope scope)
    {
        Name = name;
        Type = type;
        Scope = scope;
    }

    public string Name { get; }
    public SymbolType Type { get; }
    public Scope Scope { get; }
}