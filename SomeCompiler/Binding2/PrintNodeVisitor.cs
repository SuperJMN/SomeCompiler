using System.Text;

namespace SomeCompiler.Binding2;

public class PrintNodeVisitor : INodeVisitor
{
    private StringBuilder resultBuilder = new StringBuilder();
    private int indentationLevel = 0;

    public void VisitDeclarationNode(DeclarationNode node)
    {
        resultBuilder.AppendLine(new string('\t', indentationLevel) + $"{node.Scope.Get(node.Name).Value};");
    }

    public void VisitBlockNode(BlockNode node)
    { 
        resultBuilder.AppendLine(new string('\t', indentationLevel) + "{");
        indentationLevel++;
        foreach (var statement in node.Statements)
        {
            statement.Accept(this);
        }
        indentationLevel--;
        resultBuilder.AppendLine(new string('\t', indentationLevel) + "}");
    }

    public void VisitFunctionNode(FunctionNode node)
    {
        resultBuilder.AppendLine($"void {node.Name}()");
        node.Block.Accept(this);
    }

    public void VisitProgramNode(ProgramNode node)
    {
        foreach (var function in node.Functions)
        {
            function.Accept(this);
        }
    }

    public void VisitAssignment(AssignmentNode assignmentNode)
    {
        assignmentNode.Left.Accept(this);
        resultBuilder.Append("=");
        assignmentNode.Right.Accept(this);
    }

    public void VisitConstant(ConstantNode constantNode)
    {
        resultBuilder.Append(constantNode.Value);
    }

    public void VisitExpressionStatement(ExpressionStatementNode expressionStatementNode)
    {
        resultBuilder.Append(new string('\t', indentationLevel));
        expressionStatementNode.Expression.Accept(this);
        resultBuilder.AppendLine(";");
    }

    public void VisitKnownSymbol(KnownSymbolNode knownSymbolNode)
    {
        resultBuilder.Append(knownSymbolNode.Symbol.Name);
    }

    public void VisitUnknownSymbol(UnknownSymbol unknownSymbol)
    {
        resultBuilder.Append($"<Unknown '{unknownSymbol}' 😕>");
    }

    public void VisitAddition(AddExpressionNode addExpressionNode)
    {
        addExpressionNode.Left.Accept(this);
        resultBuilder.Append("+");
        addExpressionNode.Right.Accept(this);
    }

    public void VisitSymbolExpression(SymbolExpressionNode symbolExpressionNode)
    {
        symbolExpressionNode.SymbolNode.Accept(this);
    }

    public void VisitBinaryExpression(BinaryExpressionNode binaryExpressionNode)
    {
        binaryExpressionNode.Left.Accept(this);
        resultBuilder.Append(binaryExpressionNode.Symbol);
        binaryExpressionNode.Right.Accept(this);
    }

    public override string ToString()
    {
        return resultBuilder.ToString();
    }
}

