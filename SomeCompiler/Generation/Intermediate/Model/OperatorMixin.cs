using SomeCompiler.Compilation.Model;

namespace SomeCompiler.Generation.Intermediate.Model;

public static class OperatorMixin
{
    public static Operator ToOperator(this BinaryOperator binaryOperator)
    {
        var dict = new Dictionary<BinaryOperator, Operator>
        {
            {BinaryOperator.Add, Operator.Add},
            {BinaryOperator.Divide, Operator.Divide},
            {BinaryOperator.Multiply, Operator.Multiply},
            {BinaryOperator.Subtract, Operator.Subtract}
        };

        return dict[binaryOperator];
    }
}