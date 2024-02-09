using SomeCompiler.Binding.Model;
using SomeCompiler.Parser.Model;

namespace SomeCompiler.Binding;

public class Binder
{
    private List<Error> errors = new();

    public Result<CompiledProgram, List<Error>> Compile(Program source)
    {
        errors = new List<Error>();
        var declarations = GetFunctionDeclarations(source.Functions);
        ValidateDeclarations(declarations);

        var scope = new BinderScope();

        var functions = source.Functions.Select(function => Bind(function, scope));
        if (errors.Any())
        {
            return Result.Failure<CompiledProgram, List<Error>>(errors);
        }

        return new CompiledProgram(functions);
    }

    private BoundFunction Bind(Function function, BinderScope parentScope)
    {
        var scope = parentScope.CreateChild();
        var boundFunction = new BoundFunction(new ReturnType("int"), function.Name, Bind(function.Block, scope));
        return boundFunction;
    }

    private BoundBlock Bind(Block block, BinderScope scope)
    {
        return new BoundBlock(block.Select(statement => Bind(statement, scope)));
    }

    private BoundStatement Bind(Statement statement, BinderScope parentScope)
    {
        return statement switch
        {
            ExpressionStatement expr => Bind(expr),
            ReturnStatement ret => Bind(ret),
            DeclarationStatement decl => Bind(decl, parentScope),
            _ => throw new ArgumentOutOfRangeException(nameof(statement))
        };
    }
    
    private BoundStatement Bind(DeclarationStatement declarationStatement, BinderScope scope)
    {
        scope.Declare(declarationStatement.Name, new SymbolType(BoundType.Parse(declarationStatement.ArgumentType.ToString())));
        return new BoundDeclaration(declarationStatement.ArgumentType, declarationStatement.Name);
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
            case AndExpression:
                return new BoundAndExpression(left, right);
            case OrExpression:
                return new BoundOrExpression(left, right);
            default:
                throw new ArgumentOutOfRangeException(nameof(arithmeticBinaryOperation));
        }
    }

    private BoundExpression Bind(AssignmentExpression assignmentExpression)
    {
        return new BoundAssignmentExpression(Bind(assignmentExpression.Left), Bind(assignmentExpression.Right));
    }

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