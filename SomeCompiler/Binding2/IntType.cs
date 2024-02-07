using CSharpFunctionalExtensions;

namespace SomeCompiler.Binding2;

public record IntType : SymbolType
{
    public static readonly IntType Instance = new();
}

public class SemanticError : SemanticNode
{
    public string Message { get; }
    public SemanticError(string message)
    {
        Message = message;
    }
}