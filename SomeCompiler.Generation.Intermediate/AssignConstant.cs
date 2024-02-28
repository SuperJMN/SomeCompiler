using SomeCompiler.SemanticAnalysis;

namespace SomeCompiler.Generation.Intermediate;

public class AssignConstant : Code
{
    public Reference Reference { get; }
    public ConstantNode Constant { get; }

    public AssignConstant(Reference reference, ConstantNode constant)
    {
        Reference = reference;
        Constant = constant;
    }

    public override T Accept<T>(ICodeVisitor<T> visitor) => visitor.VisitAssignConstant(this);
}