using SomeCompiler.Core;

namespace SomeCompiler.Parser.Model;

public abstract record BinaryExpression(Expression Left, Expression Right) : Expression, IHasPrecedence
{
    public override string ToString()
    {
        return $"{FormatSubexpresion(Left)} {Symbol} {FormatSubexpresion(Right)}";
    }

    private string FormatSubexpresion(Expression expr)
    {
        if (expr is IHasPrecedence pred)
        {
            if (pred.Precedence > Precedence)
            {
                return $"({expr})";
            }
        }

        return expr.ToString();
    }

    public abstract string Symbol { get; }
    public abstract int Precedence { get; }
}