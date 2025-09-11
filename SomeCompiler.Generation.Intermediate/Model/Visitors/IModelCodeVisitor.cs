using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Generation.Intermediate.Model.Visitors;

public interface IModelCodeVisitor<out T>
{
    // Arithmetic and logic
    T VisitAdd(SomeCompiler.Generation.Intermediate.Model.Codes.Add code);
    T VisitSubtract(SomeCompiler.Generation.Intermediate.Model.Codes.Subtract code);
    T VisitMultiply(SomeCompiler.Generation.Intermediate.Model.Codes.Multiply code);
    T VisitDivide(SomeCompiler.Generation.Intermediate.Model.Codes.Divide code);
    T VisitAnd(SomeCompiler.Generation.Intermediate.Model.Codes.And code);
    T VisitOr(SomeCompiler.Generation.Intermediate.Model.Codes.Or code);

    // Assignment and movement
    T VisitAssign(SomeCompiler.Generation.Intermediate.Model.Codes.Assign code);
    T VisitAssignConstant(SomeCompiler.Generation.Intermediate.Model.Codes.AssignConstant code);
    T VisitAssignFromReturn(SomeCompiler.Generation.Intermediate.Model.Codes.AssignFromReturn code);

    // Control flow and labels
    T VisitLabel(SomeCompiler.Generation.Intermediate.Model.Codes.Label code);
    T VisitLocalLabel(SomeCompiler.Generation.Intermediate.Model.Codes.LocalLabel code);
    T VisitBranchIfZero(SomeCompiler.Generation.Intermediate.Model.Codes.BranchIfZero code);
    T VisitBranchIfNotZero(SomeCompiler.Generation.Intermediate.Model.Codes.BranchIfNotZero code);
    T VisitJump(SomeCompiler.Generation.Intermediate.Model.Codes.Jump code);

    // Calls and returns
    T VisitCall(SomeCompiler.Generation.Intermediate.Model.Codes.Call code);
    T VisitReturn(SomeCompiler.Generation.Intermediate.Model.Codes.Return code);
    T VisitEmptyReturn(SomeCompiler.Generation.Intermediate.Model.Codes.EmptyReturn code);

    // Misc/target-specific helpers in current IR
    T VisitHalt(SomeCompiler.Generation.Intermediate.Model.Codes.Halt code);
    T VisitLoadHLImm(SomeCompiler.Generation.Intermediate.Model.Codes.LoadHLImm code);
    T VisitLoadHLRef(SomeCompiler.Generation.Intermediate.Model.Codes.LoadHLRef code);
    T VisitParam(SomeCompiler.Generation.Intermediate.Model.Codes.Param code);
    T VisitParamConst(SomeCompiler.Generation.Intermediate.Model.Codes.ParamConst code);
    T VisitCleanArgs(SomeCompiler.Generation.Intermediate.Model.Codes.CleanArgs code);

    // Fallback for future codes not yet modeled explicitly
    T VisitDefault(SomeCompiler.Generation.Intermediate.Model.Codes.Code code);
}
