using RetroSharp.Generation.Intermediate.Model.Codes;

namespace RetroSharp.Generation.Intermediate.Model.Visitors;

public interface IModelCodeVisitor<out T>
{
    // Arithmetic and logic
    T VisitAdd(RetroSharp.Generation.Intermediate.Model.Codes.Add code);
    T VisitSubtract(RetroSharp.Generation.Intermediate.Model.Codes.Subtract code);
    T VisitMultiply(RetroSharp.Generation.Intermediate.Model.Codes.Multiply code);
    T VisitDivide(RetroSharp.Generation.Intermediate.Model.Codes.Divide code);
    T VisitAnd(RetroSharp.Generation.Intermediate.Model.Codes.And code);
    T VisitOr(RetroSharp.Generation.Intermediate.Model.Codes.Or code);

    // Assignment and movement
    T VisitAssign(RetroSharp.Generation.Intermediate.Model.Codes.Assign code);
    T VisitAssignConstant(RetroSharp.Generation.Intermediate.Model.Codes.AssignConstant code);
    T VisitAssignFromReturn(RetroSharp.Generation.Intermediate.Model.Codes.AssignFromReturn code);

    // Control flow and labels
    T VisitLabel(RetroSharp.Generation.Intermediate.Model.Codes.Label code);
    T VisitLocalLabel(RetroSharp.Generation.Intermediate.Model.Codes.LocalLabel code);
    T VisitBranchIfZero(RetroSharp.Generation.Intermediate.Model.Codes.BranchIfZero code);
    T VisitBranchIfNotZero(RetroSharp.Generation.Intermediate.Model.Codes.BranchIfNotZero code);
    T VisitJump(RetroSharp.Generation.Intermediate.Model.Codes.Jump code);

    // Calls and returns
    T VisitCall(RetroSharp.Generation.Intermediate.Model.Codes.Call code);
    T VisitReturn(RetroSharp.Generation.Intermediate.Model.Codes.Return code);
    T VisitEmptyReturn(RetroSharp.Generation.Intermediate.Model.Codes.EmptyReturn code);

    // Misc/target-specific helpers in current IR
    T VisitHalt(RetroSharp.Generation.Intermediate.Model.Codes.Halt code);
    T VisitLoadHLImm(RetroSharp.Generation.Intermediate.Model.Codes.LoadHLImm code);
    T VisitLoadHLRef(RetroSharp.Generation.Intermediate.Model.Codes.LoadHLRef code);
    T VisitParam(RetroSharp.Generation.Intermediate.Model.Codes.Param code);
    T VisitParamConst(RetroSharp.Generation.Intermediate.Model.Codes.ParamConst code);
    T VisitCleanArgs(RetroSharp.Generation.Intermediate.Model.Codes.CleanArgs code);

    // Fallback for future codes not yet modeled explicitly
    T VisitDefault(RetroSharp.Generation.Intermediate.Model.Codes.Code code);
}
