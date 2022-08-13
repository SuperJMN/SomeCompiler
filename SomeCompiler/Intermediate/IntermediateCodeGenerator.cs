using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using SomeCompiler.Compilation;
using SomeCompiler.Compilation.Model;

namespace SomeCompiler.Intermediate;

public class IntermediateCodeGenerator
{
    public Result<IntermediateCodeProgram, List<Error>> Generate(CompiledProgram compiledProgram)
    {
        var block = compiledProgram.Functions.First().Block;

        object? value = null;
        foreach (var statement in block)
        {
            if (statement is BoundReturnStatement rs)
            {
                value = (rs.Expression as BoundConstantExpression)?.Value;
            }
        }
        
        IEnumerable<IntermediateCode> instructions = new []
        {
            Call("main"),
            Halt(),
            
        }.Concat(GenerateFunction(compiledProgram.Functions.First(x => x.Name == "main")));
        
        return Result.Success<IntermediateCodeProgram, List<Error>>(new IntermediateCodeProgram(instructions));
    }

    private IEnumerable<IntermediateCode> GenerateFunction(BoundFunction function)
    {        
        object? value = null;
        foreach (var statement in function.Block)
        {
            if (statement is BoundReturnStatement rs)
            {
                value = (rs.Expression as BoundConstantExpression)?.Value;
            }
        }

        yield return Label(function.Name);
        yield return value is null ? Return() : Return(value);
    }

    private IntermediateCode Return(object constant)
    {
        return new ReturnCode(constant);
    }

    private IntermediateCode Return()
    {
        return new ReturnCode(Maybe<object>.None);
    }

    private IntermediateCode Label(string name)
    {
        return new LabelCode(name);
    }

    private IntermediateCode Halt()
    {
        return new HaltCode();
    }

    private IntermediateCode Call(string label)
    {
        return new CallCode(label);
    }
}