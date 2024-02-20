﻿using System.Text;

namespace SomeCompiler.Parser;

public class PrintNodeVisitor : ISyntaxVisitor
{
    private readonly StringBuilder resultBuilder = new();
    private int indentationLevel = 0;


    public override string ToString()
    {
        return resultBuilder.ToString();
    }

    public void VisitBlock(BlockSyntax block)
    {
        resultBuilder.AppendLine(new string('\t', indentationLevel) + "{");
        indentationLevel++;
        foreach (var statement in block.Statements)
        {
            statement.Accept(this);
        }
        indentationLevel--;
        resultBuilder.AppendLine(new string('\t', indentationLevel) + "}");
    }

    public void VisitProgram(ProgramSyntax programSyntax)
    {
        foreach (var function in programSyntax.Functions)
        {
            function.Accept(this);
        }
    }

    public void VisitFunctionCall(FunctionCall functionCall)
    {
        resultBuilder.Append(functionCall.Name + "()");
    }

    public void VisitMult(MultExpression multExpression)
    {
        multExpression.Left.Accept(this);
        resultBuilder.Append("*");
        multExpression.Right.Accept(this);
    }

    public void VisitIdentifierLValue(IdentifierLValue identifierLValue)
    {
        resultBuilder.Append(identifierLValue.Identifier);
    }

    public void VisitAdd(AddExpression addExpression)
    {
        addExpression.Left.Accept(this);
        resultBuilder.Append("+");
        addExpression.Right.Accept(this);
    }

    public void VisitAssignment(AssignmentSyntax assignmentSyntax)
    {
        assignmentSyntax.Left.Accept(this);
        resultBuilder.Append("=");
        assignmentSyntax.Right.Accept(this);
    }

    public void VisitExpressionStatement(ExpressionStatementSyntax expressionStatementSyntax)
    {
        resultBuilder.Append(new string('\t', indentationLevel));
        expressionStatementSyntax.Expression.Accept(this);
        resultBuilder.AppendLine(";");
    }

    public void VisitFunction(FunctionSyntax function)
    {
        resultBuilder.AppendLine($"{function.Type} {function.Name}()");
        function.Block.Accept(this);
    }
}