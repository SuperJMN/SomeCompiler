using SomeCompiler.Parser.Model;

namespace SomeCompiler.Parser.Antlr4;

public record RelationalExpression(Expression Left, Expression Right, RelationalOperator Operator) : Expression
{
    public override IEnumerable<INode> Children
    {
        get
        {
            yield return Left;
            yield return Right;
        }
    }
    
    public override string ToString()
    {
        string operatorStr = Operator switch
        {
            RelationalOperator.Equal => "==",
            RelationalOperator.NotEqual => "!=",
            RelationalOperator.Less => "<",
            RelationalOperator.Greater => ">",
            RelationalOperator.GreaterEqual => ">=",
            RelationalOperator.LessEqual=> "<=",
            _ => throw new NotImplementedException($"No se ha implementado la conversión a string para el operador {Operator}")
        };
        
        return $"{Left.ToString()} {operatorStr} {Right.ToString()}";
    }
}