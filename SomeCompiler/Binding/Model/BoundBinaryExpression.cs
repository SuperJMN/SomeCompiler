//using SomeCompiler.Core;

//namespace SomeCompiler.Binding.Model;

//public abstract record BoundBinaryExpression(BoundExpression Left, BoundExpression Right) : BoundExpression, IHasPrecedence
//{
//    public override string ToString()
//    {
//        return $"{FormatSubexpresion(Left)} {Symbol} {FormatSubexpresion(Right)}";
//    }

//    private string FormatSubexpresion(BoundExpression expr)
//    {
//        if (expr is IHasPrecedence pred)
//        {
//            if (pred.Precedence > Precedence)
//            {
//                return $"({expr})";
//            }
//        }

//        return expr.ToString();
//    }

//    public abstract string Symbol { get; }
//    public abstract int Precedence { get; }
//}