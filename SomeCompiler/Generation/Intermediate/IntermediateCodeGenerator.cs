using CSharpFunctionalExtensions;
using SomeCompiler.Compilation;
using SomeCompiler.Compilation.Model;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Generation.Intermediate;

public class IntermediateCodeGenerator
{
    public Fragment Generate(BoundAssignmentExpression assignment)
    {
        var reference = new NamedReference(assignment.Left.Identifier);
        var expressionFragment = Generate(assignment.Right);

        Code code = new Assign(reference, expressionFragment.Reference);
        return new Fragment(reference, expressionFragment.Codes.Concat(new[] {code}));
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
        if (boundReturnStatement.Expression.HasNoValue)
        {
            return new[] {Code.Return()};
        }

        var gen = Generate(boundReturnStatement.Expression.Value);
        return gen.Codes.Concat(new[] {Code.Return(gen.Reference)});
    }

    private Fragment Generate(BoundConstantExpression cex)
    {
        return new Fragment(x => new AssignConstant(x, (int)cex.Value));
    }

    private Fragment Generate(BoundBinaryExpression bex)
    {
        var left = Generate(bex.Left);
        var right = Generate(bex.Right);

        return new Fragment(reference => new Add(reference, left.Reference, right.Reference))
            .Prepend(right)
            .Prepend(left);
    }

    private Fragment Generate(BoundIdentifierExpression bex)
    {
        return new Fragment(reference => new Assign(reference, new NamedReference(bex.Identifier)));
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
        var codes2 = Generate(function.CompoundStatement);
        var codes3 = ArraySegment<Code>.Empty;

        return codes1.Concat(codes2).Concat(codes3);
    }

    private IEnumerable<Code> Generate(BoundCompoundStatement compoundStatement)
    {
        var generated = compoundStatement.Statements.SelectMany(Generate).ToList();

        AddReturnCodeIfMissing(generated);

        return generated;
    }

    private static void AddReturnCodeIfMissing(ICollection<Code> generated)
    {
        if (generated.LastOrDefault() is Return || generated.LastOrDefault() is EmptyReturn)
        {
            return;
        }

        generated.Add(new EmptyReturn());
    }

    private IEnumerable<Code> Generate(BoundStatement statement)
    {
        return statement switch
        {
            BoundExpressionStatement expressionStatement => Generate(expressionStatement).Codes,
            BoundReturnStatement boundReturnStatement => Generate(boundReturnStatement),
            _ => throw new ArgumentOutOfRangeException(nameof(statement))
        };
    }

    private Fragment Generate(BoundExpressionStatement boundExpressionStatement)
    {
        return boundExpressionStatement.Expression switch
        {
            BoundAssignmentExpression boundAssignmentExpression => Generate(boundAssignmentExpression),
            BoundBinaryExpression boundBinaryExpression => Generate(boundBinaryExpression),
            BoundConstantExpression boundConstantExpression => Generate(boundConstantExpression),
            BoundIdentifierExpression boundIdentifierExpression => Generate(boundIdentifierExpression),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}