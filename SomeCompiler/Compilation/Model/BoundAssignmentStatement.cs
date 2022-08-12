using SomeCompiler.Parsing.Model;

namespace SomeCompiler.Compilation;

internal class BoundAssignmentStatement : BoundStatement
{
    public LeftValue LeftValue { get; }
    public BoundExpression RightValue { get; }

    public BoundAssignmentStatement(LeftValue leftValue, BoundExpression rightValue)
    {
        LeftValue = leftValue;
        RightValue = rightValue;
    }

    public override string ToString() => $"{LeftValue} = {RightValue};";
}