using Antlr4.Runtime.Tree;
using DynamicData;
using SomeCompiler.Parser.Model;
using SomeCompiler.Parser.Model.Helpers;

namespace SomeCompiler.Parser.Antlr4;

public class ExpressionConverter
{
    public Expression ParseExpression(CParser.ExpressionContext expr)
    {
        return ParseAssignment((CParser.AssignmentExpressionContext) expr.GetChild(0));
    }

    private Expression ParseAssignment(CParser.AssignmentExpressionContext expr)
    {
        if (expr.ChildCount > 1)
        {
            var unary = Unary((CParser.UnaryExpressionContext) expr.GetChild(0));
            var lvalue = new LeftValue(unary.ToString());
            var right = ParseAssignment((CParser.AssignmentExpressionContext) expr.GetChild(2));
            return new AssignmentExpression(lvalue, right);
        }

        return ParseConditionalExpression((CParser.ConditionalExpressionContext) expr.GetChild(0));
    }

    private Expression ParseConditionalExpression(CParser.ConditionalExpressionContext node)
    {
        return ParseSomething((CParser.LogicalOrExpressionContext) node.GetChild(0));
    }

    private Expression ParseSomething(CParser.LogicalOrExpressionContext node)
    {
        return ParseLogicalAnd((CParser.LogicalAndExpressionContext) node.GetChild(0));
    }

    private Expression ParseLogicalAnd(CParser.LogicalAndExpressionContext node)
    {
        return ParseInclusiveOr((CParser.InclusiveOrExpressionContext) node.GetChild(0));
    }

    private Expression ParseInclusiveOr(CParser.InclusiveOrExpressionContext node)
    {
        return ParseExclusive((CParser.ExclusiveOrExpressionContext)node.GetChild(0));
    }

    private Expression ParseExclusive(CParser.ExclusiveOrExpressionContext exclusiveOrExpressionContext)
    {
        return AndExpression((CParser.AndExpressionContext) exclusiveOrExpressionContext.GetChild(0));
    }

    private Expression AndExpression(CParser.AndExpressionContext exclusiveOrExpressionContext)
    {
        return EqualityExpr((CParser.EqualityExpressionContext) exclusiveOrExpressionContext.GetChild(0));
    }

    private Expression EqualityExpr(CParser.EqualityExpressionContext node)
    {
        return Relational((CParser.RelationalExpressionContext) node.GetChild(0));
    }

    private Expression Relational(CParser.RelationalExpressionContext node)
    {
        return ShiftExpression((CParser.ShiftExpressionContext) node.GetChild(0));
    }

    private Expression ShiftExpression(CParser.ShiftExpressionContext node)
    {
        return Additive((CParser.AdditiveExpressionContext) node.GetChild(0));
    }

    private Expression Additive(CParser.AdditiveExpressionContext node)
    {
        if (node.ChildCount > 1)
        {
            return FromPostfixListAdditive(node.Children().ToList());
        }

        return Multiplicative((CParser.MultiplicativeExpressionContext) node.GetChild(0));
    }

    private string GetOp(ITerminalNode terminal)
    {
        return terminal.GetText();
    }

    private Expression Multiplicative(CParser.MultiplicativeExpressionContext node)
    {
        if (node.ChildCount > 1)
        {
            return FromPostfixList(node.Children().ToList());
        }

        return Cast((CParser.CastExpressionContext) node.GetChild(0));
    }

    private Expression? FromPostfixList(List<IParseTree> nodes)
    {
        var binaryTree = BinaryTreeHelper.Build(nodes.ToList());
        var expr = ToMultiplicativeExpression(binaryTree!);
        return expr;
    }

    private Expression? FromPostfixListAdditive(List<IParseTree> nodes)
    {
        var binaryTree = BinaryTreeHelper.Build(nodes.ToList());
        var expr = ToAdditiveExpression(binaryTree!);
        return expr;
    }

    private ArithmeticOperation? ToMultiplicativeExpression(BinaryNode<IParseTree>? binaryTree)
    {
        if (binaryTree is null)
        {
            return null;
        }

        var op = binaryTree.Value is ITerminalNode terminal ? GetOp(terminal) : null;
        
        var left = Cast((CParser.CastExpressionContext) binaryTree.Left.Value);

        Expression right;
        if (binaryTree.Right.Value is ITerminalNode)
        {
            right = ToMultiplicativeExpression(binaryTree.Right);
        }
        else
        {
            right = Cast((CParser.CastExpressionContext) binaryTree.Right.Value);
        }
        
        return new ArithmeticOperation(op, left, right);
    }

    private ArithmeticOperation? ToAdditiveExpression(BinaryNode<IParseTree>? binaryTree)
    {
        if (binaryTree is null)
        {
            return null;
        }

        var op = binaryTree.Value is ITerminalNode terminal ? GetOp(terminal) : null;
        
        var left = Multiplicative((CParser.MultiplicativeExpressionContext) binaryTree.Left.Value);

        Expression right;
        if (binaryTree.Right.Value is ITerminalNode)
        {
            right = ToAdditiveExpression(binaryTree.Right);
        }
        else
        {
            right = Multiplicative((CParser.MultiplicativeExpressionContext) binaryTree.Right.Value);
        }
        
        return new ArithmeticOperation(op, left, right);
    }

    private Expression Cast(CParser.CastExpressionContext node)
    {
        return Unary((CParser.UnaryExpressionContext) node.GetChild(0));
    }

    private Expression Unary(CParser.UnaryExpressionContext node)
    {
        if (node.ChildCount > 1)
        {
            var op = UnaryOperator((CParser.UnaryOperatorContext) node.GetChild(0));
            return new UnaryExpression(op, Cast((CParser.CastExpressionContext) node.GetChild(1)));
        }

        return Postfix((CParser.PostfixExpressionContext) node.GetChild(0));
    }

    private string UnaryOperator(CParser.UnaryOperatorContext getChild)
    {
        return getChild.GetText();
    }

    private Expression Postfix(CParser.PostfixExpressionContext node)
    {
        return ParsePrimary((CParser.PrimaryExpressionContext) node.GetChild(0));
    }

    private Expression ParsePrimary(CParser.PrimaryExpressionContext node)
    {
        var terminalNode = (ITerminalNode)node.GetChild(0);

        return terminalNode.Symbol.Type switch
        {
            111 => new ConstantExpression(int.Parse(terminalNode.GetText())),
            110 => new IdentifierExpression(terminalNode.GetText()),
            _ => throw new NotSupportedException()
        };
    }
}