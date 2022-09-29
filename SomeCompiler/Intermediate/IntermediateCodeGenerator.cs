using CSharpFunctionalExtensions;
using SomeCompiler.Compilation;
using SomeCompiler.Compilation.Model;
using SomeCompiler.Intermediate.Model;

namespace SomeCompiler.Intermediate;

public class IntermediateCodeGenerator
{
    public Fragment Generate(AssignmentExpression assignment)
    {
        var reference = new NamedReference(assignment.Identifier.Identifier);
        var expressionFragment = Generate(assignment.Expression);

        var code = new Code(reference, expressionFragment.Reference, null, Operator.Equal);
        return new Fragment(reference, expressionFragment.Codes.Concat(new[] {code}));
    }

    private Fragment Generate(ConstantExpression cex)
    {
        return new Fragment(x => new Code(x, new ConstantReference(cex.Constant), null, Operator.Equal));
    }

    private Fragment Generate(BinaryExpression bex)
    {
        var left = Generate(bex.Left);
        var right = Generate(bex.Right);

        return new Fragment(reference => new Code(reference, left.Reference, right.Reference, bex.Operator))
            .Prepend(right)
            .Prepend(left);
    }

    private Fragment Generate(IdentifierExpression bex)
    {
        return new Fragment(reference => new Code(reference, new NamedReference(bex.Identifier), null, Operator.Equal));
    }

    private Fragment Generate(Expression expression)
    {
        return expression switch
        {
            BinaryExpression bex => Generate(bex),
            ConstantExpression cex => Generate(cex),
            IdentifierExpression iex => Generate(iex),
            _ => throw new NotSupportedException()
        };
    }

    public Result<IntermediateCodeProgram,List<Error>> Generate(CompiledProgram assignment)
    {
        throw new NotImplementedException();
    }
}