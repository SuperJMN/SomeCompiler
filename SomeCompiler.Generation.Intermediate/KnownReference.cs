using SomeCompiler.SemanticAnalysis;

namespace SomeCompiler.Generation.Intermediate;

public class KnownReference : Reference
{
    public Symbol Symbol { get; }

    public KnownReference(Symbol symbol)
    {
        Symbol = symbol;
    }

    public override T Accept<T>(ICodeVisitor<T> toStringVisitor) => toStringVisitor.VisitKnownReference(this);
}