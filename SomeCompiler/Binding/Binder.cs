﻿using SomeCompiler.Binding.Model;
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
            ArithmeticOperation arithmeticOperation => Bind(arithmeticOperation),
            AssignmentExpression assignmentExpression => Bind(assignmentExpression),
            ConstantExpression constantExpression => new BoundConstantExpression(constantExpression.Value),
            IdentifierExpression identifierExpression => new BoundIdentifierExpression(identifierExpression.Identifier),
            _ => throw new ArgumentOutOfRangeException(nameof(expression))
        };
    }

    private BoundExpression Bind(ArithmeticOperation arithmeticOperation)
    {
        var left = Bind(arithmeticOperation.Expressions[0]);
        var right = Bind(arithmeticOperation.Expressions[1]);
        return new BoundBinaryExpression(left, right, GetOperator(arithmeticOperation.Op));
    }

    private static BinaryOperator GetOperator(string? term)
    {
        if (term is null)
        {
            throw new NullReferenceException(term);
        }

        return term switch
        {
            "+" => BinaryOperator.Add,
            "-" => BinaryOperator.Subtract,
            "*" => BinaryOperator.Multiply,
            "/" => BinaryOperator.Divide,
            _ => throw new NotSupportedException(term)
        };
    }

    private BoundExpression Bind(AssignmentExpression assignmentExpression)
    {
        return new BoundAssignmentExpression(Bind(assignmentExpression.Left), Bind(assignmentExpression.Right));
    }

    // Acabo de meter los métodos para LValue y assigment expression. Nada funciona. Ir rellenando hasta que se pueda y luego chutar los tests.
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