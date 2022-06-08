using CSharpFunctionalExtensions;
using SomeCompiler.Compilation.Model;
using SomeCompiler.Parsing.Model;

namespace SomeCompiler.Compilation;

public class Compiler
{
    private List<Error> errors = new();

    public Result<CompiledProgram, List<Error>> Compile(Program source)
    {
        errors = new();
        var declarations = GetFunctionDeclarations(source.Functions);
        CheckDeclarations(declarations);

        if (errors.Any())
        {
            return Result.Failure<CompiledProgram, List<Error>>(errors);
        }
        
        return Result.Success<CompiledProgram, List<Error>>(new CompiledProgram(declarations));
    }

    private void CheckDeclarations(IEnumerable<FunctionDeclaration> declarations)
    {
        declarations
            .GroupBy(x => x.Name)
            .Where(x => x.Count() > 1)
            .ToList()
            .ForEach(d => errors.Add(new Error(ErrorKind.FunctionAlreadyDeclared ,$"Function '{d.Key}' has been declared more than once")));
    }

    private List<FunctionDeclaration> GetFunctionDeclarations(Functions sourceFunctions)
    {
        return sourceFunctions.Select(GetFunctionDeclaration).ToList();
    }

    private FunctionDeclaration GetFunctionDeclaration(Function function)
    {
        return new FunctionDeclaration(function.Identifier);
    }
}

public enum ErrorKind
{
    FunctionAlreadyDeclared
}

public class Error
{
    public ErrorKind Kind { get; }
    public string Message { get; }

    public Error(ErrorKind kind, string message)
    {
        Kind = kind;
        Message = message;
    }
}

public class FunctionDeclaration
{
    public string Name { get; }

    public FunctionDeclaration(string name)
    {
        Name = name;
    }
}