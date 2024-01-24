using Antlr4.Runtime.Tree;
using CSharpFunctionalExtensions;
using SomeCompiler.Core.Helpers;
using SomeCompiler.Parser.Model;

namespace SomeCompiler.Parser.Antlr4;

public class ExpressionConverter
{
    public Expression ParseExpression(CParser.ExpressionContext expr)
    {
        return ParseAssignment((CParser.AssignmentExpressionContext)expr.GetChild(0));
    }

    private Expression ParseAssignment(CParser.AssignmentExpressionContext expr)
    {
        if (expr.ChildCount > 1)
        {
            var unary = Unary((CParser.UnaryExpressionContext)expr.GetChild(0));
            var lvalue = new LeftValue(unary.ToString());
            var right = ParseAssignment((CParser.AssignmentExpressionContext)expr.GetChild(2));
            return new AssignmentExpression(lvalue, right);
        }

        return ParseConditionalExpression((CParser.ConditionalExpressionContext)expr.GetChild(0));
    }

    private Expression ParseConditionalExpression(CParser.ConditionalExpressionContext node)
    {
        return LogicalOr((CParser.LogicalOrExpressionContext)node.GetChild(0));
    }

    private Expression LogicalOr(CParser.LogicalOrExpressionContext node)
    {
        if (node.ChildCount > 1)
        {
            var binaryTree = BinaryTreeHelper.FromPostFix(node.Children().ToList());
            return ToExpression(binaryTree!, tree => LogicalAnd((CParser.LogicalAndExpressionContext)tree));
        }

        return LogicalAnd((CParser.LogicalAndExpressionContext)node.GetChild(0));
    }

    private Expression LogicalAnd(CParser.LogicalAndExpressionContext node)
    {
        if (node.ChildCount > 1)
        {
            var binaryTree = BinaryTreeHelper.FromPostFix(node.Children().ToList());
            return ToExpression(binaryTree!, tree => InclusiveOr((CParser.InclusiveOrExpressionContext)tree));
        }

        return InclusiveOr((CParser.InclusiveOrExpressionContext)node.GetChild(0));
    }

    private Expression InclusiveOr(CParser.InclusiveOrExpressionContext node)
    {
        return ExclusiveOr((CParser.ExclusiveOrExpressionContext)node.GetChild(0));
    }

    private Expression ExclusiveOr(CParser.ExclusiveOrExpressionContext exclusiveOrExpressionContext)
    {
        return And((CParser.AndExpressionContext)exclusiveOrExpressionContext.GetChild(0));
    }

    private Expression And(CParser.AndExpressionContext andExpressionContext)
    {
        return EqualityExpr((CParser.EqualityExpressionContext)andExpressionContext.GetChild(0));
    }

    private Expression EqualityExpr(CParser.EqualityExpressionContext node)
    {
        return Relational((CParser.RelationalExpressionContext)node.GetChild(0));
    }

    private Expression Relational(CParser.RelationalExpressionContext node)
    {
        var left = Shift(node.shiftExpression(0));
        var right = Maybe.From(node.shiftExpression(1)).Map(Shift);
        if (node.Greater().Length > 0)
        {
            return CreateRelationalExpression(left, right.Value, RelationalOperator.Greater);
        }
        if (node.Less(0) != null)
        {
            return CreateRelationalExpression(left, right.Value, RelationalOperator.Less);
        }
        if (node.LessEqual(0) != null)
        {
            return CreateRelationalExpression(left, right.Value, RelationalOperator.LessEqual);
        }
        if (node.GreaterEqual(0) != null)
        {
            return CreateRelationalExpression(left, right.Value, RelationalOperator.GreaterEqual);
        }
        return Shift(node.shiftExpression(0));
    }
    
    private static RelationalExpression CreateRelationalExpression(Expression left, Expression right, RelationalOperator @operator)
    {
        return new RelationalExpression(left, right, @operator);
    }
    
    private Expression Shift(CParser.ShiftExpressionContext node)
    {
        return Additive((CParser.AdditiveExpressionContext)node.GetChild(0));
    }

    private Expression Additive(CParser.AdditiveExpressionContext node)
    {
        if (node.ChildCount > 1)
        {
            var binaryTree = BinaryTreeHelper.FromPostFix(node.Children().ToList());
            return ToExpression(binaryTree!, tree => Multiplicative((CParser.MultiplicativeExpressionContext)tree));
        }

        return Multiplicative((CParser.MultiplicativeExpressionContext)node.GetChild(0));
    }

    private Expression Multiplicative(CParser.MultiplicativeExpressionContext node)
    {
        if (node.ChildCount > 1)
        {
            var binaryTree = BinaryTreeHelper.FromPostFix(node.Children().ToList().ToList());
            return ToExpression(binaryTree!, tree => Cast((CParser.CastExpressionContext)tree));
        }

        return Cast((CParser.CastExpressionContext)node.GetChild(0));
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

        var right = convertExpression(binaryTree.Right!.Value);

        Expression left;
        if (binaryTree.Left!.Value is ITerminalNode)
        {
            left = ToExpression(binaryTree.Left, convertExpression);
        }
        else
        {
            left = convertExpression(binaryTree.Left.Value);
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
                "&&" => new AndExpression(left, right),
                "||" => new OrExpression(left, right),
                _ => throw new ArgumentOutOfRangeException(t.GetText())
            };
        }

        throw new ArgumentOutOfRangeException(binaryTree.Value.GetText());
    }

    private Expression Cast(CParser.CastExpressionContext node)
    {
        return Unary((CParser.UnaryExpressionContext)node.GetChild(0));
    }

    private Expression Unary(CParser.UnaryExpressionContext node)
    {
        if (node.ChildCount > 1)
        {
            var op = UnaryOperator((CParser.UnaryOperatorContext)node.GetChild(0));
            return new UnaryExpression(op, Cast((CParser.CastExpressionContext)node.GetChild(1)));
        }

        return Postfix((CParser.PostfixExpressionContext)node.GetChild(0));
    }

    private string UnaryOperator(CParser.UnaryOperatorContext getChild)
    {
        return getChild.GetText();
    }

    private Expression Postfix(CParser.PostfixExpressionContext node)
    {
        return ParsePrimary((CParser.PrimaryExpressionContext)node.GetChild(0));
    }

    private Expression ParsePrimary(CParser.PrimaryExpressionContext node)
    {
        var terminalNode = (ITerminalNode)node.GetChild(0);

        return terminalNode.Symbol.Type switch
        {
            111 => new ConstantExpression(int.Parse(terminalNode.GetText())),
            110 => new IdentifierExpression(terminalNode.GetText()),
            64 => ParseExpression((CParser.ExpressionContext)node.GetChild(1)),
            _ => throw new NotSupportedException()
        };
    }
}