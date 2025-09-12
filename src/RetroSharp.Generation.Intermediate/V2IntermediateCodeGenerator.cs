using System.Globalization;
using RetroSharp.Generation.Intermediate.Model;
using RetroSharp.SemanticAnalysis;

namespace RetroSharp.Generation.Intermediate;

public class V2IntermediateCodeGenerator
{
    private int labelCounter = 0;

    public RetroSharp.Generation.Intermediate.Model.IntermediateCodeProgram Generate(ProgramNode program)
    {
        var codes = new List<RetroSharp.Generation.Intermediate.Model.Codes.Code>();
        foreach (var function in program.Functions)
        {
            // Function label
            codes.Add(new RetroSharp.Generation.Intermediate.Model.Codes.Label(function.Name));
            // Encode parameter names for backend layout
            if (function.Parameters.Count > 0)
            {
                var tag = $"{function.Name}::__params__:{string.Join(",", function.Parameters)}";
                codes.Add(new RetroSharp.Generation.Intermediate.Model.Codes.LocalLabel(tag));
            }
            var body = GenerateBlock(function.Name, function.Block).ToList();
            codes.AddRange(body);

            // Ensure a return for each function
            if (codes.Count == 0 || codes[^1] is not RetroSharp.Generation.Intermediate.Model.Codes.Return && codes[^1] is not RetroSharp.Generation.Intermediate.Model.Codes.EmptyReturn)
            {
                codes.Add(new RetroSharp.Generation.Intermediate.Model.Codes.EmptyReturn());
            }
        }

        return new RetroSharp.Generation.Intermediate.Model.IntermediateCodeProgram(codes);
    }

    private IEnumerable<RetroSharp.Generation.Intermediate.Model.Codes.Code> GenerateBlock(string functionName, BlockNode block)
    {
        foreach (var statement in block.Statements)
        {
            foreach (var code in GenerateStatement(functionName, statement))
            {
                yield return code;
            }
        }
    }

    private IEnumerable<RetroSharp.Generation.Intermediate.Model.Codes.Code> GenerateStatement(string functionName, StatementNode statement)
    {
        switch (statement)
        {
            case DeclarationNode:
                // Declarations don't emit code in this minimal IR
                return Enumerable.Empty<RetroSharp.Generation.Intermediate.Model.Codes.Code>();
            case ExpressionStatementNode es:
                return GenerateExpression(functionName, es.Expression).codes;
            case ReturnNode ret:
                return EmitReturn(functionName, ret);
            case IfElseNode ie:
                return EmitIfElse(functionName, ie);
            default:
                throw new NotSupportedException($"Statement not supported: {statement.GetType().Name}");
        }
    }

    private IEnumerable<RetroSharp.Generation.Intermediate.Model.Codes.Code> EmitReturn(string functionName, ReturnNode ret)
    {
        if (ret.Expression.HasValue)
        {
            var expr = GenerateExpression(functionName, ret.Expression.Value);
            var codes = new List<RetroSharp.Generation.Intermediate.Model.Codes.Code>();
            codes.AddRange(expr.codes);
            codes.Add(new RetroSharp.Generation.Intermediate.Model.Codes.Return(expr.r));
            return codes;
        }
        else
        {
            return new RetroSharp.Generation.Intermediate.Model.Codes.Code[]
            {
                new RetroSharp.Generation.Intermediate.Model.Codes.EmptyReturn()
            };
        }
    }

    private IEnumerable<RetroSharp.Generation.Intermediate.Model.Codes.Code> EmitIfElse(string functionName, IfElseNode ie)
    {
        var elseLabel = new RetroSharp.Generation.Intermediate.Model.Codes.LocalLabel($"{functionName}_ELSE_{labelCounter}");
        var endLabel = new RetroSharp.Generation.Intermediate.Model.Codes.LocalLabel($"{functionName}_END_{labelCounter}");
        labelCounter++;

        var codes = new List<RetroSharp.Generation.Intermediate.Model.Codes.Code>();

        if (ie.Condition is BinaryExpressionNode be && (be.Operator.Symbol == "==" || be.Operator.Symbol == "!="))
        {
            var left = GenerateExpression(functionName, be.Left);
            var right = GenerateExpression(functionName, be.Right);
            var diff = new RetroSharp.Generation.Intermediate.Model.Placeholder();
            codes.AddRange(left.codes);
            codes.AddRange(right.codes);
            codes.Add(new RetroSharp.Generation.Intermediate.Model.Codes.Subtract(diff, left.r, right.r));
            if (be.Operator.Symbol == "==")
            {
                // if (left == right) then ... else ...
                // Branch to else when diff != 0
                codes.Add(new RetroSharp.Generation.Intermediate.Model.Codes.BranchIfNotZero(diff, elseLabel.Name));
            }
            else
            {
                // if (left != right) then ... else ...
                // Branch to else when diff == 0
                codes.Add(new RetroSharp.Generation.Intermediate.Model.Codes.BranchIfZero(diff, elseLabel.Name));
            }
        }
        else
        {
            var cond = GenerateExpression(functionName, ie.Condition);
            codes.AddRange(cond.codes);
            // Generic: zero means false -> branch to else
            codes.Add(new RetroSharp.Generation.Intermediate.Model.Codes.BranchIfZero(cond.r, elseLabel.Name));
        }

        // then
        codes.AddRange(GenerateBlock(functionName, ie.Then));
        codes.Add(new RetroSharp.Generation.Intermediate.Model.Codes.Jump(endLabel.Name));
        // else
        codes.Add(elseLabel);
        if (ie.Else.HasValue)
        {
            codes.AddRange(GenerateBlock(functionName, ie.Else.Value));
        }
        codes.Add(endLabel);
        return codes;
    }

    private (CodeGeneration.Model.Classes.Reference r, List<RetroSharp.Generation.Intermediate.Model.Codes.Code> codes) GenerateExpression(string functionName, ExpressionNode expr)
    {
        switch (expr)
        {
            case ConstantNode c:
                return EmitConstant(c);
            case SymbolExpressionNode se:
                return EmitSymbol(functionName, se);
            case BinaryExpressionNode be:
                return EmitBinary(functionName, be);
            case AssignmentNode assign:
                return EmitAssignment(functionName, assign);
            case FunctionCallExpressionNode fcall:
                return EmitFunctionCall(functionName, fcall);
            default:
                throw new NotSupportedException($"Expression not supported: {expr.GetType().Name}");
        }
    }

    private (CodeGeneration.Model.Classes.Reference r, List<RetroSharp.Generation.Intermediate.Model.Codes.Code> codes) EmitConstant(ConstantNode c)
    {
        var target = new RetroSharp.Generation.Intermediate.Model.Placeholder();
        var value = CoerceInt(c.Value);
        return (target, new List<RetroSharp.Generation.Intermediate.Model.Codes.Code> { new RetroSharp.Generation.Intermediate.Model.Codes.AssignConstant(target, value) });
    }

    private (CodeGeneration.Model.Classes.Reference r, List<RetroSharp.Generation.Intermediate.Model.Codes.Code> codes) EmitSymbol(string functionName, SymbolExpressionNode se)
    {
        var target = new RetroSharp.Generation.Intermediate.Model.Placeholder();
        var baseName = se.SymbolNode switch
        {
            KnownSymbolNode ks => ks.Symbol.Name,
            UnknownSymbol us => us.Name,
            _ => se.SymbolNode.ToString()
        };
        var qualified = $"{functionName}::{baseName}";
        return (target, new List<RetroSharp.Generation.Intermediate.Model.Codes.Code> { new RetroSharp.Generation.Intermediate.Model.Codes.Assign(target, new RetroSharp.Generation.Intermediate.Model.NamedReference(qualified)) });
    }

    private (CodeGeneration.Model.Classes.Reference r, List<RetroSharp.Generation.Intermediate.Model.Codes.Code> codes) EmitBinary(string functionName, BinaryExpressionNode be)
    {
        var left = GenerateExpression(functionName, be.Left);
        var right = GenerateExpression(functionName, be.Right);
        var target = new RetroSharp.Generation.Intermediate.Model.Placeholder();

        var op = be.Operator.Symbol; // e.g. +, -, *, /
        RetroSharp.Generation.Intermediate.Model.Codes.Code opCode = op switch
        {
            "+" => new RetroSharp.Generation.Intermediate.Model.Codes.Add(target, left.r, right.r),
            "-" => new RetroSharp.Generation.Intermediate.Model.Codes.Subtract(target, left.r, right.r),
            "*" => new RetroSharp.Generation.Intermediate.Model.Codes.Multiply(target, left.r, right.r),
            "/" => new RetroSharp.Generation.Intermediate.Model.Codes.Divide(target, left.r, right.r),
            _ => throw new NotSupportedException($"Operator '{op}' not supported")
        };

        var codes = new List<RetroSharp.Generation.Intermediate.Model.Codes.Code>();
        codes.AddRange(left.codes);
        codes.AddRange(right.codes);
        codes.Add(opCode);
        return (target, codes);
    }

    private (CodeGeneration.Model.Classes.Reference r, List<RetroSharp.Generation.Intermediate.Model.Codes.Code> codes) EmitAssignment(string functionName, AssignmentNode a)
    {
        var rhs = GenerateExpression(functionName, a.Right);
        var baseName = a.Left switch
        {
            KnownSymbolNode ks => ks.Symbol.Name,
            UnknownSymbol us => us.Name,
            _ => a.Left.ToString()
        };
        var qualified = $"{functionName}::{baseName}";
        var to = new RetroSharp.Generation.Intermediate.Model.NamedReference(qualified);
        var codes = new List<RetroSharp.Generation.Intermediate.Model.Codes.Code>();
        codes.AddRange(rhs.codes);
        codes.Add(new RetroSharp.Generation.Intermediate.Model.Codes.Assign(to, rhs.r));
        return (to, codes);
    }

    private (CodeGeneration.Model.Classes.Reference r, List<RetroSharp.Generation.Intermediate.Model.Codes.Code> codes) EmitFunctionCall(string functionName, FunctionCallExpressionNode fcall)
    {
        var codes = new List<RetroSharp.Generation.Intermediate.Model.Codes.Code>();
        var argOps = new List<object>(); // either int (immediate) or Reference

        foreach (var arg in fcall.Arguments)
        {
            if (arg is ConstantNode c)
            {
                var imm = CoerceInt(c.Value);
                argOps.Add(imm);
            }
            else
            {
                var a = GenerateExpression(functionName, arg);
                codes.AddRange(a.codes);
                argOps.Add(a.r);
            }
        }
        // push all args to stack in order (no more special HL handling for first param)
        for (int i = 0; i < argOps.Count; i++)
        {
            var op = argOps[i];
            if (op is int imm)
            {
                codes.Add(new RetroSharp.Generation.Intermediate.Model.Codes.ParamConst(imm));
            }
            else
            {
                codes.Add(new RetroSharp.Generation.Intermediate.Model.Codes.Param((CodeGeneration.Model.Classes.Reference)op));
            }
        }
        codes.Add(new RetroSharp.Generation.Intermediate.Model.Codes.Call(fcall.Name, argOps.Count));
        // Clean pushed args (all args are now pushed)
        var pushed = argOps.Count;
        if (pushed > 0)
        {
            codes.Add(new RetroSharp.Generation.Intermediate.Model.Codes.CleanArgs(pushed));
        }
        var target = new RetroSharp.Generation.Intermediate.Model.Placeholder();
        codes.Add(new RetroSharp.Generation.Intermediate.Model.Codes.AssignFromReturn(target));
        return (target, codes);
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
