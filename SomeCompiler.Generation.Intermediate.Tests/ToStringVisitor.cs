using Zafiro.Core.Mixins;

namespace SomeCompiler.Generation.Intermediate.Tests;

public class ToStringVisitor : ICodeVisitor<string>
{
    private int placeholderCount;
    private readonly Dictionary<PlaceholderReference, string> placeholders = new();

    public string VisitCall(Call callInstruction)
    {
        // Suponiendo que la clase Call tiene una propiedad Name
        return $"Call {callInstruction.Function.Name}";
    }
    public string VisitFunctionCode(FunctionCode functionCodeInstruction)
    {
        // Suponiendo que la clase FunctionCode tiene una propiedad Name
        return $"Function {functionCodeInstruction.Function.Name}:";
    }
    public string VisitHalt(Halt haltInstruction)
    {
        // Suponiendo que la clase Halt tiene una propiedad Name
        return $"Halt";
    }
    public string VisitBinaryExpression(BinaryExpressionCode binaryExpressionInstruction)
    {
        // Suponiendo que la clase BinaryExpressionCode tiene una propiedad Name
        return $"BinaryExpression: {binaryExpressionInstruction.Target.Accept(this)} = {binaryExpressionInstruction.LeftReference.Accept(this)} {binaryExpressionInstruction.Operator.Symbol} {binaryExpressionInstruction.RightReference.Accept(this)}";
    }
    public string VisitAssignReference(AssignReference assignReferenceInstruction)
    {
        // Suponiendo que la clase AssignReference tiene una propiedad Name
        return $"AssignReference: {assignReferenceInstruction.Target.Accept(this)} = {assignReferenceInstruction.Source.Accept(this)}";
    }
    public string VisitAssignConstant(AssignConstant assignConstantInstruction)
    {
        // Suponiendo que la clase AssignConstant tiene una propiedad Name
        return $"AssignConstant: {assignConstantInstruction.Reference.Accept(this)} = {assignConstantInstruction.Constant.Value}";
    }
    public string VisitProgram(IntermediateCodeProgram program)
    {
        var enumerable = program.Select(code =>
            {
                var accept = code.Accept(this);
                return accept;
            })
            .ToList();
        return enumerable.JoinWithLines();
    }

    public string VisitKnownReference(KnownReference knownReference) => knownReference.Symbol.Name;

    public string VisitPlaceholderReference(PlaceholderReference placeholderReference) => placeholders.GetCreate(placeholderReference, () => "T" + ++placeholderCount);
}