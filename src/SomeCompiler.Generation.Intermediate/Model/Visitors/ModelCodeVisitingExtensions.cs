using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Generation.Intermediate.Model.Visitors;

public static class ModelCodeVisitingExtensions
{
    public static T Accept<T>(this SomeCompiler.Generation.Intermediate.Model.Codes.Code code, IModelCodeVisitor<T> visitor)
        => code switch
        {
            SomeCompiler.Generation.Intermediate.Model.Codes.Add x => visitor.VisitAdd(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.Subtract x => visitor.VisitSubtract(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.Multiply x => visitor.VisitMultiply(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.Divide x => visitor.VisitDivide(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.And x => visitor.VisitAnd(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.Or x => visitor.VisitOr(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.Assign x => visitor.VisitAssign(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.AssignConstant x => visitor.VisitAssignConstant(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.AssignFromReturn x => visitor.VisitAssignFromReturn(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.Label x => visitor.VisitLabel(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.LocalLabel x => visitor.VisitLocalLabel(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.BranchIfZero x => visitor.VisitBranchIfZero(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.BranchIfNotZero x => visitor.VisitBranchIfNotZero(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.Jump x => visitor.VisitJump(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.Call x => visitor.VisitCall(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.Return x => visitor.VisitReturn(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.EmptyReturn x => visitor.VisitEmptyReturn(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.Halt x => visitor.VisitHalt(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.LoadHLImm x => visitor.VisitLoadHLImm(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.LoadHLRef x => visitor.VisitLoadHLRef(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.Param x => visitor.VisitParam(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.ParamConst x => visitor.VisitParamConst(x),
            SomeCompiler.Generation.Intermediate.Model.Codes.CleanArgs x => visitor.VisitCleanArgs(x),
            _ => visitor.VisitDefault(code)
        };
}
