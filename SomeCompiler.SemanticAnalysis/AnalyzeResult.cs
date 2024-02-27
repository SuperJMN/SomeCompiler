namespace SomeCompiler.SemanticAnalysis;

public class AnalyzeResult<T>(T node, Scope scope)
{
    public T Node { get; } = node;
    public Scope Scope { get; } = scope;
}