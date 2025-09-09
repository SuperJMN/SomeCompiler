using System.Globalization;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.SemanticAnalysis;

namespace SomeCompiler.Generation.Intermediate;

public class V2IntermediateCodeGenerator
{
    public SomeCompiler.Generation.Intermediate.Model.IntermediateCodeProgram Generate(ProgramNode program)
    {
        var codes = new List<SomeCompiler.Generation.Intermediate.Model.Codes.Code>();
        foreach (var function in program.Functions)
        {
            // Function label
            codes.Add(new SomeCompiler.Generation.Intermediate.Model.Codes.Label(function.Name));
            var body = GenerateBlock(function.Block).ToList();
            codes.AddRange(body);

            // Ensure a return of some kind
            if (codes.Count == 0 || codes[^1] is not SomeCompiler.Generation.Intermediate.Model.Codes.Return && codes[^1] is not SomeCompiler.Generation.Intermediate.Model.Codes.EmptyReturn)
            {
                codes.Add(new SomeCompiler.Generation.Intermediate.Model.Codes.EmptyReturn());
            }
        }

        return new SomeCompiler.Generation.Intermediate.Model.IntermediateCodeProgram(codes);
    }

    private IEnumerable<SomeCompiler.Generation.Intermediate.Model.Codes.Code> GenerateBlock(BlockNode block)
    {
        foreach (var statement in block.Statements)
        {
            foreach (var code in GenerateStatement(statement))
            {
                yield return code;
            }
        }
    }

    private IEnumerable<SomeCompiler.Generation.Intermediate.Model.Codes.Code> GenerateStatement(StatementNode statement)
    {
        switch (statement)
        {
            case DeclarationNode:
                // Declarations don't emit code in this minimal IR
                return Enumerable.Empty<SomeCompiler.Generation.Intermediate.Model.Codes.Code>();
            case ExpressionStatementNode es:
                return GenerateExpression(es.Expression).codes;
            default:
                throw new NotSupportedException($"Statement not supported: {statement.GetType().Name}");
        }
    }

    private (CodeGeneration.Model.Classes.Reference r, List<SomeCompiler.Generation.Intermediate.Model.Codes.Code> codes) GenerateExpression(ExpressionNode expr)
    {
        switch (expr)
        {
            case ConstantNode c:
                return EmitConstant(c);
            case SymbolExpressionNode se:
                return EmitSymbol(se);
            case BinaryExpressionNode be:
                return EmitBinary(be);
            case AssignmentNode assign:
                return EmitAssignment(assign);
            default:
                throw new NotSupportedException($"Expression not supported: {expr.GetType().Name}");
        }
    }

    private (CodeGeneration.Model.Classes.Reference r, List<SomeCompiler.Generation.Intermediate.Model.Codes.Code> codes) EmitConstant(ConstantNode c)
    {
        var target = new SomeCompiler.Generation.Intermediate.Model.Placeholder();
        var value = CoerceInt(c.Value);
        return (target, new List<SomeCompiler.Generation.Intermediate.Model.Codes.Code> { new SomeCompiler.Generation.Intermediate.Model.Codes.AssignConstant(target, value) });
    }

    private (CodeGeneration.Model.Classes.Reference r, List<SomeCompiler.Generation.Intermediate.Model.Codes.Code> codes) EmitSymbol(SymbolExpressionNode se)
    {
        var target = new SomeCompiler.Generation.Intermediate.Model.Placeholder();
        var name = se.SymbolNode switch
        {
            KnownSymbolNode ks => ks.Symbol.Name,
            UnknownSymbol us => us.Name,
            _ => se.SymbolNode.ToString()
        };
        return (target, new List<SomeCompiler.Generation.Intermediate.Model.Codes.Code> { new SomeCompiler.Generation.Intermediate.Model.Codes.Assign(target, new SomeCompiler.Generation.Intermediate.Model.NamedReference(name)) });
    }

    private (CodeGeneration.Model.Classes.Reference r, List<SomeCompiler.Generation.Intermediate.Model.Codes.Code> codes) EmitBinary(BinaryExpressionNode be)
    {
        var left = GenerateExpression(be.Left);
        var right = GenerateExpression(be.Right);
        var target = new SomeCompiler.Generation.Intermediate.Model.Placeholder();

        var op = be.Operator.Symbol; // e.g. +, -, *, /
        SomeCompiler.Generation.Intermediate.Model.Codes.Code opCode = op switch
        {
            "+" => new SomeCompiler.Generation.Intermediate.Model.Codes.Add(target, left.r, right.r),
            "-" => new SomeCompiler.Generation.Intermediate.Model.Codes.Subtract(target, left.r, right.r),
            "*" => new SomeCompiler.Generation.Intermediate.Model.Codes.Multiply(target, left.r, right.r),
            "/" => new SomeCompiler.Generation.Intermediate.Model.Codes.Divide(target, left.r, right.r),
            _ => throw new NotSupportedException($"Operator '{op}' not supported")
        };

        var codes = new List<SomeCompiler.Generation.Intermediate.Model.Codes.Code>();
        codes.AddRange(left.codes);
        codes.AddRange(right.codes);
        codes.Add(opCode);
        return (target, codes);
    }

    private (CodeGeneration.Model.Classes.Reference r, List<SomeCompiler.Generation.Intermediate.Model.Codes.Code> codes) EmitAssignment(AssignmentNode a)
    {
        var rhs = GenerateExpression(a.Right);
        var name = a.Left switch
        {
            KnownSymbolNode ks => ks.Symbol.Name,
            UnknownSymbol us => us.Name,
            _ => a.Left.ToString()
        };
        var to = new SomeCompiler.Generation.Intermediate.Model.NamedReference(name);
        var codes = new List<SomeCompiler.Generation.Intermediate.Model.Codes.Code>();
        codes.AddRange(rhs.codes);
        codes.Add(new SomeCompiler.Generation.Intermediate.Model.Codes.Assign(to, rhs.r));
        return (to, codes);
    }

    private static int CoerceInt(object value)
    {
        if (value is int i) return i;
        if (value is string s)
        {
            if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var vi)) return vi;
            if (bool.TryParse(s, out var vb)) return vb ? 1 : 0;
        }
        if (value is bool b) return b ? 1 : 0;
        throw new NotSupportedException($"Constant value '{value}' not supported");
    }
}
