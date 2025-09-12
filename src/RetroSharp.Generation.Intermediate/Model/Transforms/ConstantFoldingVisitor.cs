using CSharpFunctionalExtensions;
using RetroSharp.Generation.Intermediate.Model.Codes;
using RetroSharp.Generation.Intermediate.Model.Visitors;

namespace RetroSharp.Generation.Intermediate.Model.Transforms;

using ModelCode = RetroSharp.Generation.Intermediate.Model.Codes.Code;
using Ref = CodeGeneration.Model.Classes.Reference;

public class ConstantFoldingVisitor : IIntermediateTransform, RetroSharp.Generation.Intermediate.Model.Visitors.IModelCodeVisitor<RetroSharp.Generation.Intermediate.Model.Codes.Code>
{
    private readonly Dictionary<Ref, int> consts = new();

public Result<IntermediateCodeProgram> Run(IntermediateCodeProgram input)
    {
        consts.Clear();
        var output = new List<ModelCode>();
        foreach (var code in input)
        {
            var transformed = code.Accept(this);
            output.Add(transformed);
        }
        return Result.Success(new IntermediateCodeProgram(output));
    }

public ModelCode VisitAdd(RetroSharp.Generation.Intermediate.Model.Codes.Add code)
    {
        if (TryGet(code.Left, out var l) && TryGet(code.Right, out var r))
            return AssignConst(code.Target, l + r);
        Invalidate(code.Target);
        return code;
    }

public ModelCode VisitSubtract(RetroSharp.Generation.Intermediate.Model.Codes.Subtract code)
    {
        if (TryGet(code.Left, out var l) && TryGet(code.Right, out var r))
            return AssignConst(code.Target, l - r);
        Invalidate(code.Target);
        return code;
    }

public ModelCode VisitMultiply(RetroSharp.Generation.Intermediate.Model.Codes.Multiply code)
    {
        if (TryGet(code.Left, out var l) && TryGet(code.Right, out var r))
            return AssignConst(code.Target, l * r);
        Invalidate(code.Target);
        return code;
    }

public ModelCode VisitDivide(RetroSharp.Generation.Intermediate.Model.Codes.Divide code)
    {
        if (TryGet(code.Left, out var l) && TryGet(code.Right, out var r) && r != 0)
            return AssignConst(code.Target, l / r);
        Invalidate(code.Target);
        return code;
    }

public ModelCode VisitAnd(RetroSharp.Generation.Intermediate.Model.Codes.And code)
    {
        if (TryGet(code.Left, out var l) && TryGet(code.Right, out var r))
            return AssignConst(code.Target, l & r);
        Invalidate(code.Target);
        return code;
    }

public ModelCode VisitOr(RetroSharp.Generation.Intermediate.Model.Codes.Or code)
    {
        if (TryGet(code.Left, out var l) && TryGet(code.Right, out var r))
            return AssignConst(code.Target, l | r);
        Invalidate(code.Target);
        return code;
    }

public ModelCode VisitAssign(RetroSharp.Generation.Intermediate.Model.Codes.Assign code)
    {
        if (TryGet(code.Source, out var v))
        {
            consts[code.Target] = v;
return new RetroSharp.Generation.Intermediate.Model.Codes.AssignConstant(code.Target, v);
        }
        Invalidate(code.Target);
        return code;
    }

public ModelCode VisitAssignConstant(RetroSharp.Generation.Intermediate.Model.Codes.AssignConstant code)
    {
        consts[code.Target] = code.Source;
        return code;
    }

public ModelCode VisitAssignFromReturn(RetroSharp.Generation.Intermediate.Model.Codes.AssignFromReturn code)
    {
        Invalidate(code.Target);
        return code;
    }

public ModelCode VisitLabel(RetroSharp.Generation.Intermediate.Model.Codes.Label code) => code;
public ModelCode VisitLocalLabel(RetroSharp.Generation.Intermediate.Model.Codes.LocalLabel code) => code;

public ModelCode VisitBranchIfZero(RetroSharp.Generation.Intermediate.Model.Codes.BranchIfZero code) => code;
public ModelCode VisitBranchIfNotZero(RetroSharp.Generation.Intermediate.Model.Codes.BranchIfNotZero code) => code;
public ModelCode VisitJump(RetroSharp.Generation.Intermediate.Model.Codes.Jump code) => code;

public ModelCode VisitCall(RetroSharp.Generation.Intermediate.Model.Codes.Call code)
    {
        // Be conservative: calls may mutate any reference (by convention, we could scope later).
        consts.Clear();
        return code;
    }

public ModelCode VisitReturn(RetroSharp.Generation.Intermediate.Model.Codes.Return code) => code;
public ModelCode VisitEmptyReturn(RetroSharp.Generation.Intermediate.Model.Codes.EmptyReturn code) => code;
public ModelCode VisitHalt(RetroSharp.Generation.Intermediate.Model.Codes.Halt code) => code;
public ModelCode VisitLoadHLImm(RetroSharp.Generation.Intermediate.Model.Codes.LoadHLImm code) => code;
public ModelCode VisitLoadHLRef(RetroSharp.Generation.Intermediate.Model.Codes.LoadHLRef code) => code;
public ModelCode VisitParam(RetroSharp.Generation.Intermediate.Model.Codes.Param code) => code;
public ModelCode VisitParamConst(RetroSharp.Generation.Intermediate.Model.Codes.ParamConst code) => code;
public ModelCode VisitCleanArgs(RetroSharp.Generation.Intermediate.Model.Codes.CleanArgs code) => code;

public RetroSharp.Generation.Intermediate.Model.Codes.Code VisitDefault(RetroSharp.Generation.Intermediate.Model.Codes.Code code) => code;


private bool TryGet(Ref r, out int value)
        => consts.TryGetValue(r, out value);

    private void Invalidate(Ref r)
    {
        if (consts.ContainsKey(r))
            consts.Remove(r);
    }

private static ModelCode AssignConst(Ref target, int v)
        => new RetroSharp.Generation.Intermediate.Model.Codes.AssignConstant(target, v);
}
