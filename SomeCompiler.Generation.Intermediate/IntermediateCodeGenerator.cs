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
            return new Fragment(reference => new AssignReference(reference, new KnownReference(symbol.Symbol)));
        }
        
        if (expr is ConstantNode constant)
        {
            return new Fragment(reference => new AssignConstant(reference, constant));
        }

        if (expr is AssignmentNode assignment)
        {
            var reference = new KnownReference(((KnownSymbolNode)assignment.Left).Symbol);
            var expressionFragment = GenerateExpression(assignment.Right);

            Code code = new AssignReference(reference, expressionFragment.Reference);
            return new Fragment(reference, expressionFragment.Codes.Concat(new[] { code }));
        }

        throw new InvalidOperationException();
    }

    private IEnumerable<Code> GenerateDeclaration(DeclarationNode decl)
    {
        return Enumerable.Empty<Code>();
    }
}