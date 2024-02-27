using System.Runtime.InteropServices.JavaScript;
using CSharpFunctionalExtensions;
using SomeCompiler.Core;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.SemanticAnalysis;

namespace SomeCompiler.Generation.Intermediate;

public class IntermediateCodeGenerator
{
    public Result<IntermediateCodeProgram> GenerateFunction(ProgramNode compiledProgram)
    {
        var value = compiledProgram.Functions.Select(GenerateFunction).SelectMany(x => x).ToList();
        return Result.Success(new IntermediateCodeProgram(value));
    }

    //private static void AddReturnCodeIfMissing(ICollection<Code> generated)
    //{
    //    if (generated.LastOrDefault() is Return || generated.LastOrDefault() is EmptyReturn)
    //    {
    //        return;
    //    }

    //    generated.Add(new EmptyReturn());
    //}

    //private Fragment Generate(AssignmentNode assignment)
    //{
    //    var reference = new NamedReference(((KnownSymbolNode)assignment.Left).Symbol);
    //    var expressionFragment = Generate(assignment.Right);

    //    Code code = new Assign(reference, expressionFragment.Reference);
    //    return new Fragment(reference, expressionFragment.Codes.Concat(new[] { code }));
    //}

    //private IEnumerable<Code> Generate(BoundReturnStatement boundReturnStatement)
    //{
    //    if (boundReturnStatement.Expression.HasNoValue)
    //    {
    //        return new[] { new EmptyReturn() };
    //    }

    //    var gen = Generate(boundReturnStatement.Expression.Value);
    //    return gen.Codes.Concat(new[] { new Return(gen.Reference) });
    //}

    //private Fragment Generate(BoundConstantExpression cex)
    //{
    //    return new Fragment(x => new AssignConstant(x, (int) cex.Value));
    //}

    //private Fragment Generate(BoundBinaryExpression bex)
    //{
    //    var left = Generate(bex.Left);
    //    var right = Generate(bex.Right);

    //    return new Fragment(reference => GetCodeFromBinaryExpression(bex, reference, left, right))
    //        .Prepend(right)
    //        .Prepend(left);
    //}

    //private static Code GetCodeFromBinaryExpression(BoundBinaryExpression op, Reference reference, Fragment left, Fragment right)
    //{
    //    return op switch
    //    {
    //        BoundAddExpression => new Add(reference, left.Reference, right.Reference),
    //        BoundSubtractExpression => new Subtract(reference, left.Reference, right.Reference),
    //        BoundMultiplyExpression => new Multiply(reference, left.Reference, right.Reference),
    //        BoundDivideExpression => new Divide(reference, left.Reference, right.Reference),
    //        BoundAndExpression => new And(reference, left.Reference, right.Reference),
    //        BoundOrExpression => new Or(reference, left.Reference, right.Reference),
    //        _ => throw new ArgumentOutOfRangeException(nameof(op))
    //    };
    //}

    //private Fragment Generate(BoundIdentifierExpression bex)
    //{
    //    return new Fragment(reference => new Assign(reference, new NamedReference(bex.Identifier)));
    //}

    //private Fragment Generate(BoundExpression expression)
    //{
    //    return expression switch
    //    {
    //        BoundBinaryExpression bex => Generate(bex),
    //        BoundConstantExpression cex => Generate(cex),
    //        BoundIdentifierExpression iex => Generate(iex),
    //        _ => throw new NotSupportedException()
    //    };
    //}

    private IEnumerable<Code> GenerateFunction(FunctionNode function)
    {
        var label = new Code[] { new FunctionCode(function) };
        var block = GenerateBlock(function.Block);

        return label.Concat(block);
    }

    private IEnumerable<Code> GenerateBlock(BlockNode block)
    {
        return block.Statements.SelectMany(GenerateStatement);
    }

    //private IEnumerable<Code> Generate(BoundBlock block)
    //{
    //    var generated = block.Statements.SelectMany(Generate).ToList();

    //    AddReturnCodeIfMissing(generated);

    //    return generated;
    //}

    private IEnumerable<Code> GenerateStatement(StatementNode statement)
    {
        return statement switch
        {
            DeclarationNode decl => GenerateDeclaration(decl),
            ExpressionStatementNode expressionStatement => GenerateExpressionStatement(expressionStatement).Codes,
            _ => throw new ArgumentOutOfRangeException(nameof(statement))
        };
    }

    private Fragment GenerateExpressionStatement(ExpressionStatementNode expr)
    {
        return GenerateExpression(expr.Expression);
    }

    private Fragment GenerateBinaryExpression(BinaryExpressionNode binaryExpression)
    {
        var left = GenerateExpression(binaryExpression.Left);
        var right = GenerateExpression(binaryExpression.Right);
        var op = binaryExpression.Operator;
        return new Fragment(reference => GetCodeFromBinaryExpression(reference, left.Reference, right.Reference, op))
            .Prepend(right)
            .Prepend(left);
    }

    private Code GetCodeFromBinaryExpression(Reference reference, Reference left, Reference right, Operator op)
    {
        return new BinaryExpressionCode(reference, left, right, op);
    }

    private Fragment GenerateExpression(ExpressionNode expr)
    {
        if (expr is BinaryExpressionNode binaryExpr)
        {
            return GenerateBinaryExpression(binaryExpr);
        }

        if (expr is SymbolExpressionNode { SymbolNode: KnownSymbolNode { } symbol }  symbolExpr )
        {
            return new Fragment(reference => new AssignCode(symbol.Symbol.Name, reference));
        }
        
        if (expr is ConstantNode constant)
        {
            return new Fragment(reference => new AssignConstant(reference, constant));
        }

        throw new InvalidOperationException();
    }

    private IEnumerable<Code> GenerateDeclaration(DeclarationNode decl)
    {
        throw new NotImplementedException();
    }
}

internal class AssignConstant : Code
{
    public Reference Reference { get; }
    public ConstantNode Constant { get; }

    public AssignConstant(Reference reference, ConstantNode constant)
    {
        Reference = reference;
        Constant = constant;
    }
}

internal class AssignCode : Code
{
    public string SymbolName { get; }
    public Reference Reference { get; }

    public AssignCode(string symbolName, Reference reference)
    {
        SymbolName = symbolName;
        Reference = reference;
    }
}

internal class BinaryExpressionCode : Code
{
    public Reference Target { get; }
    public Reference LeftReference { get; }
    public Reference RightReference { get; }

    public BinaryExpressionCode(Reference target, Reference leftReference, Reference rightReference, Operator op)
    {
        Target = target;
        LeftReference = leftReference;
        RightReference = rightReference;
    }
}

internal class FunctionCode : Code
{
    public FunctionNode Function { get; }

    public FunctionCode(FunctionNode function)
    {
        Function = function;
    }
}

public class Halt : Code
{
}

public class Call : Code
{
    public FunctionNode Main { get; }

    public Call(FunctionNode main)
    {
        Main = main;
    }
}

public abstract class Code
{
}

public abstract class Reference;

class KnownReference : Reference
{
    public Symbol Symbol { get; }

    public KnownReference(Symbol symbol)
    {
        Symbol = symbol;
    }
}

public class PlaceholderReference : Reference
{
}

public class IntermediateCodeProgram : List<Code>
{
    public IntermediateCodeProgram(List<Code> value) : base(value)
    {
    }
}