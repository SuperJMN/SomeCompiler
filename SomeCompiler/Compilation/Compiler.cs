using CSharpFunctionalExtensions;
using SomeCompiler.Compilation.Model;
using SomeCompiler.Generation.Intermediate;
using SomeCompiler.Parsing.Model;

namespace SomeCompiler.Compilation;

public class Compiler
{
    private List<Error> errors = new();

    public Result<CompiledProgram, List<Error>> Compile(Program source)
    {
        errors = new List<Error>();
        var declarations = GetFunctionDeclarations(source.Functions);
        CheckDeclarations(declarations);
        var functions = source.Functions.Select(Bind);

        if (errors.Any()) return Result.Failure<CompiledProgram, List<Error>>(errors);

        return Result.Success<CompiledProgram, List<Error>>(new CompiledProgram(functions));
    }

    private BoundFunction Bind(Function function) => new(function.ReturnType, function.Identifier, Bind(function.CompoundStatement));

    private BoundCompoundStatement Bind(CompoundStatement compoundStatement)
    {
        var boundReturnStatements = compoundStatement.Statements.Select(Bind);
        return new BoundCompoundStatement(boundReturnStatements.ToList());
    }

    private BoundStatement Bind(Statement statement)
    {
        if (statement is ReturnStatement rs)
        {
            return new BoundReturnStatement(rs.Expression.Match(x => Bind(x), () => new Maybe<BoundExpression>()));
        }

        if (statement is ExpressionStatement expressionStatement)
        {
            return new BoundExpressionStatement(Bind(expressionStatement.Expression));
        }

        throw new NotSupportedException();
    }

    private BoundExpression Bind(ConstantExpression constantExpression)
    {
        return new BoundConstantExpression(constantExpression.Constant);
    }

    private BoundExpression Bind(AssignmentExpression assignmentExpression)
    {
        return new BoundAssignmentExpression(assignmentExpression.LeftValue, Bind(assignmentExpression.Expression));
    }

    private BoundExpression Bind(Expression expression)
    {
        return expression switch
        {
            AssignmentExpression assignmentExpression => Bind(assignmentExpression),
            ConstantExpression constantExpression => Bind(constantExpression),
            IdentifierExpression identifierExpression => throw new NotImplementedException(),
            NegateExpression negateExpression => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void CheckDeclarations(IList<FunctionDeclaration> declarations)
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

    private static FunctionDeclaration GetFunctionDeclaration(Function function) => new(function.Identifier);
}