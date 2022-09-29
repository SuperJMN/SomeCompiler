using CSharpFunctionalExtensions;
using SomeCompiler.Compilation;
using SomeCompiler.Compilation.Model;
using SomeCompiler.Generation.Intermediate.Model;

namespace SomeCompiler.Generation.Intermediate;

public class IntermediateCodeGenerator
{
    public IEnumerable<Code> Generate(BoundAssignmentStatement assignment)
    {
        var reference = new NamedReference(assignment.LeftValue.Identifier);
        var expressionFragment = Generate(assignment.RightValue);

        var code = new Code(reference, expressionFragment.Reference, null, Operator.Equal);
        return expressionFragment.Codes.Concat(new[] {code});
    }

    public Result<IntermediateCodeProgram, List<Error>> Generate(CompiledProgram compiledProgram)
    {
        var bootstrap = new[]
        {
            Code.Call("main"),
            Code.Halt()
        };

        var codes = compiledProgram.Functions.Select(Generate).SelectMany(x => x).ToList();

        return new IntermediateCodeProgram(bootstrap.Concat(codes));
    }

    private IEnumerable<Code> Generate(BoundReturnStatement boundReturnStatement)
    {
        if (boundReturnStatement.Expression is null)
        {
            return new[] {Code.Return()};
        }

        var gen = Generate(boundReturnStatement.Expression);
        return gen.Codes.Concat(new[] {Code.Return(gen.Reference)});
    }

    private Fragment Generate(BoundConstantExpression cex)
    {
        return new Fragment(x => new Code(x, new ConstantReference((int) cex.Value), null, Operator.Equal));
    }

    private Fragment Generate(BoundBinaryExpression bex)
    {
        var left = Generate(bex.Left);
        var right = Generate(bex.Right);

        return new Fragment(reference => new Code(reference, left.Reference, right.Reference, bex.Operator.ToOperator()))
            .Prepend(right)
            .Prepend(left);
    }

    private Fragment Generate(BoundIdentifierExpression bex)
    {
        return new Fragment(reference => new Code(reference, new NamedReference(bex.Identifier), null, Operator.Equal));
    }

    private Fragment Generate(BoundExpression expression)
    {
        return expression switch
        {
            BoundBinaryExpression bex => Generate(bex),
            BoundConstantExpression cex => Generate(cex),
            BoundIdentifierExpression iex => Generate(iex),
            _ => throw new NotSupportedException()
        };
    }

    private IEnumerable<Code> Generate(BoundFunction function)
    {
        var codes1 = new[] {Code.Label(function.Name)};
        var codes2 = Generate(function.Block);
        var codes3 = ArraySegment<Code>.Empty;

        return codes1.Concat(codes2).Concat(codes3);
    }

    private IEnumerable<Code> Generate(BoundBlock block)
    {
        var generated = block.SelectMany(Generate).ToList();

        AddReturnCodeIfMissing(generated);

        return generated;
    }

    private static void AddReturnCodeIfMissing(List<Code> generated)
    {
        if (generated.LastOrDefault()?.Operator != Operator.Return)
        {
            generated.Add(Code.Return());
        }
    }

    private IEnumerable<Code> Generate(BoundStatement statement)
    {
        return statement switch
        {
            BoundAssignmentStatement boundAssignmentStatement => Generate(boundAssignmentStatement),
            BoundReturnStatement boundReturnStatement => Generate(boundReturnStatement),
            _ => throw new ArgumentOutOfRangeException(nameof(statement))
        };
    }
}