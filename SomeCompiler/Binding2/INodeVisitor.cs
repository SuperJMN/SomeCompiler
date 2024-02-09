namespace SomeCompiler.Binding2;

public interface INodeVisitor
{
    void VisitDeclarationNode(DeclarationNode node);
    void VisitBlockNode(BlockNode node);
    void VisitFunctionNode(FunctionNode node);
    void VisitProgramNode(ProgramNode node);
    void VisitExpression(ExpressionNode expression);
    void VisitAssignment(AssignmentNode assignmentNode);
}