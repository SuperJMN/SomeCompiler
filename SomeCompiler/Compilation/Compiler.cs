using CSharpFunctionalExtensions;
using SomeCompiler.Compilation.Model;
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

    private BoundFunction Bind(Function function) =>
        new BoundFunction(function.ReturnType, function.Identifier, Bind(function.Block));

    private BoundBlock Bind(Block block)
    {
        var boundReturnStatements = block.Statements.Select(Bind);
        return new BoundBlock(boundReturnStatements.ToList());
    }

    private BoundStatement Bind(Statement statement)
    {
        if (statement is ReturnStatement rs)
        {
            return  (BoundStatement)new BoundReturnStatement(Bind(rs.Expression));
        }

        throw new NotSupportedException();
    }

    private BoundExpression Bind(Expression expression)
    {
        return new BoundConstantExpression(expression.Value);
    }

    private void CheckDeclarations(IEnumerable<FunctionDeclaration> declarations)
    {
        if (declarations.All(x => x.Name != "main"))
            errors.Add(new Error(ErrorKind.MainNotDeclared, "Main functions is not declared"));

        declarations
            .GroupBy(x => x.Name)
            .Where(x => x.Count() > 1)
            .ToList()
            .ForEach(d => errors.Add(new Error(ErrorKind.FunctionAlreadyDeclared,
                $"Function '{d.Key}' has been declared more than once")));
    }

    private List<FunctionDeclaration> GetFunctionDeclarations(Functions sourceFunctions) =>
        sourceFunctions.Select(GetFunctionDeclaration).ToList();

    private FunctionDeclaration GetFunctionDeclaration(Function function) =>
        new FunctionDeclaration(function.Identifier);
}