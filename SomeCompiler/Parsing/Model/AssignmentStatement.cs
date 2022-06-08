namespace SomeCompiler.Parsing.Model;

internal class AssignmentStatement : Statement
{
    public AssignmentStatement(LeftValue leftValue, Expression expression)
    {
        LeftValue = leftValue;
        Expression = expression;
    }

    public LeftValue LeftValue { get; }
    public Expression Expression { get; }

    public override string ToString() => $"{LeftValue} = {Expression};";
}