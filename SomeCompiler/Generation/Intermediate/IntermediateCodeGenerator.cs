using CodeGeneration.Model.Classes;
using SomeCompiler.Binding;
using SomeCompiler.Binding.Model;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Generation.Intermediate;

public class IntermediateCodeGenerator
{
    public Result<IntermediateCodeProgram, List<Error>> Generate(CompiledProgram compiledProgram)
    {
        var bootstrap = new Code[]
        {
            new Call("main"),
            new Halt()
        };

        var codes = compiledProgram.Functions.Select(Generate).SelectMany(x => x).ToList();

        return new IntermediateCodeProgram(bootstrap.Concat(codes));
    }

    private static void AddReturnCodeIfMissing(ICollection<Code> generated)
    {
        if (generated.LastOrDefault() is Return || generated.LastOrDefault() is EmptyReturn)
        {
            return;
        }

        generated.Add(new EmptyReturn());
    }

    private Fragment Generate(BoundAssignmentExpression assignment)
    {
        var reference = new NamedReference(assignment.Left.Identifier);
        var expressionFragment = Generate(assignment.Right);

        Code code = new Assign(reference, expressionFragment.Reference);
        return new Fragment(reference, expressionFragment.Codes.Concat(new[] { code }));
    }

    private IEnumerable<Code> Generate(BoundReturnStatement boundReturnStatement)
    {
        if (boundReturnStatement.Expression.HasNoValue)
        {
            return new[] { new EmptyReturn() };
        }

        var gen = Generate(boundReturnStatement.Expression.Value);
        return gen.Codes.Concat(new[] { new Return(gen.Reference) });
    }

    private Fragment Generate(BoundConstantExpression cex)
    {
        return new Fragment(x => new AssignConstant(x, (int) cex.Value));
    }

    private Fragment Generate(BoundBinaryExpression bex)
    {
        var left = Generate(bex.Left);
        var right = Generate(bex.Right);
        
        return new Fragment(reference => GetOperator(bex.Operator, reference, left, right))
            .Prepend(right)
            .Prepend(left);
    }

    private static Code GetOperator(BinaryOperator op, Reference reference, Fragment left, Fragment right)
    {
        if (op == BinaryOperator.Add)
        {
            return new Add(reference, left.Reference, right.Reference);
        }

        if (op == BinaryOperator.Subtract)
        {
            return new Subtract(reference, left.Reference, right.Reference);
        }

        if (op == BinaryOperator.Multiply)
        {
            return new Multiply(reference, left.Reference, right.Reference);
        }

        if (op == BinaryOperator.Divide)
        {
            return new Divide(reference, left.Reference, right.Reference);
        }

        throw new NotSupportedException();
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
        var label = new[] { (Code) new Label(function.Name) };
        var block = Generate(function.Block);

        return label.Concat(block);
    }

    private IEnumerable<Code> Generate(BoundBlock block)
    {
        var generated = block.Statements.SelectMany(Generate).ToList();

        AddReturnCodeIfMissing(generated);

        return generated;
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