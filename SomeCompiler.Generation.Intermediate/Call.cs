using SomeCompiler.SemanticAnalysis;

namespace SomeCompiler.Generation.Intermediate;

public class Call : Code
{
    public FunctionNode Function { get; }

    public Call(FunctionNode function)
    {
        Function = function;
    }

    public override T Accept<T>(ICodeVisitor<T> visitor)
    {
        return visitor.VisitCall(this);
    }
}