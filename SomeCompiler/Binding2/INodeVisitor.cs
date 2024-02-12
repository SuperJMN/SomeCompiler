namespace SomeCompiler.Binding2;

public interface INodeVisitor
{
    void VisitDeclarationNode(DeclarationNode node);
    void VisitBlockNode(BlockNode node);
    void VisitFunctionNode(FunctionNode node);
    void VisitProgramNode(ProgramNode node);
    void VisitAssignment(AssignmentNode assignmentNode);
    void VisitConstant(ConstantNode constantNode);
    void VisitExpressionStatement(ExpressionStatementNode expressionStatementNode);
    void VisitKnownSymbol(KnownSymbolNode knownSymbolNode);
    void VisitUnknownSymbol(UnknownSymbol unknownSymbol);
}