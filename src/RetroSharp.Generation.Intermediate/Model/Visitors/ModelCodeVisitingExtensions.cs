using RetroSharp.Generation.Intermediate.Model.Codes;

namespace RetroSharp.Generation.Intermediate.Model.Visitors;

public static class ModelCodeVisitingExtensions
{
    public static T Accept<T>(this RetroSharp.Generation.Intermediate.Model.Codes.Code code, IModelCodeVisitor<T> visitor)
        => code switch
        {
            RetroSharp.Generation.Intermediate.Model.Codes.Add x => visitor.VisitAdd(x),
            RetroSharp.Generation.Intermediate.Model.Codes.Subtract x => visitor.VisitSubtract(x),
            RetroSharp.Generation.Intermediate.Model.Codes.Multiply x => visitor.VisitMultiply(x),
            RetroSharp.Generation.Intermediate.Model.Codes.Divide x => visitor.VisitDivide(x),
            RetroSharp.Generation.Intermediate.Model.Codes.And x => visitor.VisitAnd(x),
            RetroSharp.Generation.Intermediate.Model.Codes.Or x => visitor.VisitOr(x),
            RetroSharp.Generation.Intermediate.Model.Codes.Assign x => visitor.VisitAssign(x),
            RetroSharp.Generation.Intermediate.Model.Codes.AssignConstant x => visitor.VisitAssignConstant(x),
            RetroSharp.Generation.Intermediate.Model.Codes.AssignFromReturn x => visitor.VisitAssignFromReturn(x),
            RetroSharp.Generation.Intermediate.Model.Codes.Label x => visitor.VisitLabel(x),
            RetroSharp.Generation.Intermediate.Model.Codes.LocalLabel x => visitor.VisitLocalLabel(x),
            RetroSharp.Generation.Intermediate.Model.Codes.BranchIfZero x => visitor.VisitBranchIfZero(x),
            RetroSharp.Generation.Intermediate.Model.Codes.BranchIfNotZero x => visitor.VisitBranchIfNotZero(x),
            RetroSharp.Generation.Intermediate.Model.Codes.Jump x => visitor.VisitJump(x),
            RetroSharp.Generation.Intermediate.Model.Codes.Call x => visitor.VisitCall(x),
            RetroSharp.Generation.Intermediate.Model.Codes.Return x => visitor.VisitReturn(x),
            RetroSharp.Generation.Intermediate.Model.Codes.EmptyReturn x => visitor.VisitEmptyReturn(x),
            RetroSharp.Generation.Intermediate.Model.Codes.Halt x => visitor.VisitHalt(x),
            RetroSharp.Generation.Intermediate.Model.Codes.LoadHLImm x => visitor.VisitLoadHLImm(x),
            RetroSharp.Generation.Intermediate.Model.Codes.LoadHLRef x => visitor.VisitLoadHLRef(x),
            RetroSharp.Generation.Intermediate.Model.Codes.Param x => visitor.VisitParam(x),
            RetroSharp.Generation.Intermediate.Model.Codes.ParamConst x => visitor.VisitParamConst(x),
            RetroSharp.Generation.Intermediate.Model.Codes.CleanArgs x => visitor.VisitCleanArgs(x),
            _ => visitor.VisitDefault(code)
        };
}
