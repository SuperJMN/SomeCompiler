﻿using System.Text;

namespace SomeCompiler.Binding2;

public class PrintNodeVisitor : INodeVisitor
{
    private StringBuilder resultBuilder = new StringBuilder();
    private int indentationLevel = 0;

    public void VisitDeclarationNode(DeclarationNode node)
    {
        resultBuilder.AppendLine(new string('\t', indentationLevel) + $"{node.Scope.Get(node.Name).Value} {node.Name};");
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
        indentationLevel++;
        node.Block.Accept(this);
        indentationLevel--;
    }

    public void VisitProgramNode(ProgramNode node)
    {
        foreach (var function in node.Functions)
        {
            function.Accept(this);
            resultBuilder.AppendLine();
        }
    }

    public override string ToString()
    {
        return resultBuilder.ToString();
    }
}

