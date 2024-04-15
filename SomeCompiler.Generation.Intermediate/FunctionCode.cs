using SomeCompiler.SemanticAnalysis;

namespace SomeCompiler.Generation.Intermediate;

public class FunctionCode : Code
{
    public FunctionNode Function { get; }

    public FunctionCode(FunctionNode function)
    {
        Function = function;
    }

    public override T Accept<T>(ICodeVisitor<T> visitor) => visitor.VisitFunctionCode(this);
}