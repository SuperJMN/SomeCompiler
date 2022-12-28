using SomeCompiler.Binding.Model;
using SomeCompiler.Parser.Model;
using BinaryOperator = SomeCompiler.Binding.Model.BinaryOperator;

namespace SomeCompiler.Binding;

public class Binder
{
    private List<Error> errors = new();

    public Result<CompiledProgram, List<Error>> Compile(Program source)
    {
        errors = new List<Error>();
        var declarations = GetFunctionDeclarations(source.Functions);
        ValidateDeclarations(declarations);
        var functions = source.Functions.Select(Bind);

        if (errors.Any())
        {
            return Result.Failure<CompiledProgram, List<Error>>(errors);
        }
        
        return new CompiledProgram(functions);
    }

    private BoundFunction Bind(Function function)
    {
        return new BoundFunction(new ReturnType("int"), function.Name, Bind(function.Block));
    }

    private BoundBlock Bind(Block block)
    {
        return new BoundBlock(block.Select(Bind));
    }

    private BoundStatement Bind(Statement statement)
    {
        return statement switch
        {
            ExpressionStatement expr => Bind(expr),
            ReturnStatement ret => Bind(ret),
            _ => throw new ArgumentOutOfRangeException(nameof(statement))
        };
    }

    private BoundStatement Bind(ReturnStatement returnStatement)
    {
        return new BoundReturnStatement(returnStatement.Expression.Map(Bind));
    }

    private BoundStatement Bind(ExpressionStatement expressionStatement)
    {
        return new BoundExpressionStatement(Bind(expressionStatement.Expression));
    }

    private BoundExpression Bind(Expression expression)
    {
        return expression switch
        {
            BinaryExpression arithmeticOperation => Bind(arithmeticOperation),
            AssignmentExpression assignmentExpression => Bind(assignmentExpression),
            ConstantExpression constantExpression => new BoundConstantExpression(constantExpression.Value),
            IdentifierExpression identifierExpression => new BoundIdentifierExpression(identifierExpression.Identifier),
            _ => throw new ArgumentOutOfRangeException(nameof(expression))
        };
    }

    private BoundExpression Bind(BinaryExpression arithmeticBinaryOperation)
    {
        var left = Bind(arithmeticBinaryOperation.Left);
        var right = Bind(arithmeticBinaryOperation.Right);
        switch (arithmeticBinaryOperation)
        {
            case AddExpression:
                return new BoundAddExpression(left, right);
            case DivideExpression:
                return new BoundDivideExpression(left, right);
            case MultiplyExpression:
                return new BoundMultiplyExpression(left, right);
            case SubtractExpression:
                return new BoundSubtractExpression(left, right);
            default:
                throw new ArgumentOutOfRangeException(nameof(arithmeticBinaryOperation));
        }
    }
    
    private BoundExpression Bind(AssignmentExpression assignmentExpression)
    {
        return new BoundAssignmentExpression(Bind(assignmentExpression.Left), Bind(assignmentExpression.Right));
    }

    // Acabo de meter los métodos para LValue y assignment expression. Nada funciona. Ir rellenando hasta que se pueda y luego chutar los tests.
    private LeftValue Bind(LeftValue leftValue)
    {
        return leftValue;
    }

    private void ValidateDeclarations(IList<FunctionDeclaration> declarations)
    {
        if (declarations.All(x => x.Name != "main"))
            errors.Add(new Error(ErrorKind.MainNotDeclared, "'main' function is not declared"));

        declarations
            .GroupBy(x => x.Name)
            .Where(x => x.Count() > 1)
            .ToList()
            .ForEach(d => errors.Add(new Error(ErrorKind.FunctionAlreadyDeclared,
                $"Function '{d.Key}' has been declared more than once")));
    }

    private List<FunctionDeclaration> GetFunctionDeclarations(Functions sourceFunctions) =>
    sourceFunctions.Select(GetFunctionDeclaration).ToList();

    private static FunctionDeclaration GetFunctionDeclaration(Function function) => new(function.Name);
}

