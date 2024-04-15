namespace SomeCompiler.Generation.Intermediate;

public interface ICodeVisitor<out T>
{
    T VisitCall(Call callInstruction);
    T VisitFunctionCode(FunctionCode functionCodeInstruction);
    T VisitHalt(Halt haltInstruction);
    T VisitBinaryExpression(BinaryExpressionCode binaryExpressionInstruction);
    T VisitAssignReference(AssignReference assignReferenceInstruction);
    T VisitAssignConstant(AssignConstant assignConstantInstruction);
    T VisitProgram(IntermediateCodeProgram program);
    T VisitKnownReference(KnownReference knownReference);
    T VisitPlaceholderReference(PlaceholderReference placeholderReference);
}