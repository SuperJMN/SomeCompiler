using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model;

public record Code(Reference? Target, Reference? Left, Reference? Right, Operator Operator)
{
    public static Code Halt() => new Code(null, null, null, Operator.Halt);
    public static Code Return(Reference? reference = null) => new Code(reference, null, null, Operator.Return);
    public static Code Add(Reference target, Reference left, Reference right) => new Code(target, left, right, Operator.Add);
    public static Code Multiply(Reference target, Reference left, Reference right) => new Code(target, left, right, Operator.Multiply);
    public static Code Subtract(Reference target, Reference left, Reference right) => new Code(target, left, right, Operator.Subtract);
    public static Code Divide(Reference target, Reference left, Reference right) => new Code(target, left, right, Operator.Divide);
    public static Code Label(string name) => new Code(new LabelReference(name), null, null, Operator.Label);
    public static Code Call(string name) => new Code(new LabelReference(name), null, null, Operator.Call);
}