using SomeCompiler.Core;

namespace SomeCompiler.Generation.Intermediate;

public class BinaryExpressionCode : Code
{
    public Reference Target { get; }
    public Reference LeftReference { get; }
    public Reference RightReference { get; }
    public Operator Operator { get; }

    public BinaryExpressionCode(Reference target, Reference leftReference, Reference rightReference, Operator @operator)
    {
        Target = target;
        LeftReference = leftReference;
        RightReference = rightReference;
        Operator = @operator;
    }

    public override T Accept<T>(ICodeVisitor<T> visitor) => visitor.VisitBinaryExpression(this);
}