using RetroSharp.Generation.Intermediate.Model.Codes;

namespace RetroSharp.Z80.Core;

public class Z80AssemblyGenerator
{
    private readonly IntermediateEmitter intermediateEmitter;

    public Z80AssemblyGenerator(IntermediateEmitter intermediateEmitter)
    {
        this.intermediateEmitter = intermediateEmitter;
    }

    public IEnumerable<string> Generate(Code code)
    {
        return code switch
        {
            Add add => intermediateEmitter.Addition(add),
            Assign assign => intermediateEmitter.Assign(assign),
            AssignConstant assignConstant => intermediateEmitter.AssignConstant(assignConstant),
            Call call => intermediateEmitter.Call(call),
            Divide divide => intermediateEmitter.Divide(divide),
            EmptyReturn emptyReturn => intermediateEmitter.EmptyReturn(),
            Halt halt => intermediateEmitter.Halt(),
            Multiply multiply => intermediateEmitter.Multiply(multiply),
            Return ret => intermediateEmitter.Return(ret),
            Subtract subtract => intermediateEmitter.Subtraction(subtract),
            Label label => new[] { $"{label.Name}:"},
            Param param => intermediateEmitter.Param(param),
            ParamConst pconst => intermediateEmitter.ParamConst(pconst),
            LoadHLImm lhi => intermediateEmitter.LoadHLImm(lhi),
            LoadHLRef lhr => intermediateEmitter.LoadHLRef(lhr),
            AssignFromReturn afr => intermediateEmitter.AssignFromReturn(afr),
            Jump jump => intermediateEmitter.Jump(jump),
            BranchIfZero brz => intermediateEmitter.BranchIfZero(brz),
            BranchIfNotZero brnz => intermediateEmitter.BranchIfNotZero(brnz),
            RetroSharp.Generation.Intermediate.Model.Codes.CleanArgs clean => intermediateEmitter.CleanArgs(clean),
            LocalLabel localLabel => new[] { $"{localLabel.Name}:"},
            _ => throw new ArgumentOutOfRangeException(nameof(code))
        };
    }
}