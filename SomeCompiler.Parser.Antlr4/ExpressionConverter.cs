using System.Runtime.Intrinsics.Arm;
using Antlr4.Runtime.Tree;
using SomeCompiler.Core.Helpers;
using SomeCompiler.Parser.Model;

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
            var binaryTree = BinaryTreeHelper.FromPostFix(node.Children().ToList());
            return ToExpression(binaryTree!, tree => Multiplicative((CParser.MultiplicativeExpressionContext) tree));        }

        return Multiplicative((CParser.MultiplicativeExpressionContext) node.GetChild(0));
    }

    private Expression Multiplicative(CParser.MultiplicativeExpressionContext node)
    {
        if (node.ChildCount > 1)
        {
            var binaryTree = BinaryTreeHelper.FromPostFix(node.Children().ToList().ToList());
            return ToExpression(binaryTree!, tree => Cast((CParser.CastExpressionContext)tree));
        }

        return Cast((CParser.CastExpressionContext) node.GetChild(0));
    }

    private Expression ToExpression(BinaryNode<IParseTree> binaryTree, Func<IParseTree, Expression> convertExpression)
    {
        if (binaryTree == null)
        {
            throw new ArgumentNullException(nameof(binaryTree));
        }

        if (convertExpression == null)
        {
            throw new ArgumentNullException(nameof(convertExpression));
        }

        var left = convertExpression(binaryTree.Left!.Value);

        Expression right;
        if (binaryTree.Right!.Value is ITerminalNode)
        {
            right = ToExpression(binaryTree.Right, convertExpression);
        }
        else
        {
            right = convertExpression(binaryTree.Right.Value);
        }

        return GetExpr(binaryTree, left, right);
    }

    private static Expression GetExpr(BinaryNode<IParseTree> binaryTree, Expression left, Expression right)
    {
        if (binaryTree.Value is ITerminalNode t)
        {
            return t.GetText() switch
            {
                "+" => new AddExpression(left, right),
                "-" => new SubtractExpression(left, right),
                "*" => new MultiplyExpression(left, right),
                "/" => new DivideExpression(left, right),
                _ => throw new ArgumentOutOfRangeException(t.GetText())
            };
        }

        throw new ArgumentOutOfRangeException(binaryTree.Value.GetText());
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
            64 => ParseExpression((CParser.ExpressionContext) node.GetChild(1)),
            _ => throw new NotSupportedException()
        };
    }
}
