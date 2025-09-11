using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Z80.Core;

public class IntermediateEmitter
{
    private readonly OpCodeEmitter opCodeEmitter;
    private int parameterCount;
    private int frameSize;

    public IntermediateEmitter(OpCodeEmitter opCodeEmitter, int parameterCount = 0, int frameSize = 0)
    {
        this.opCodeEmitter = opCodeEmitter;
        this.parameterCount = parameterCount;
        this.frameSize = frameSize;
    }

    public IEnumerable<string> AssignConstant(AssignConstant assignConstant)
    {
        var lines = new List<string>();
        lines.Add(opCodeEmitter.Set(assignConstant.Source, Register.HL));
        // If target is a Placeholder used to load first arg into HL, we don't need to store back.
        // Detect by absence in table: in that case, just leave HL loaded.
        if (opCodeEmitter.HasReference(assignConstant.Target))
        {
            lines.AddRange(opCodeEmitter.Set(Register.HL, assignConstant.Target));
        }
        return lines;
    }

    public IEnumerable<string> Addition(Add add)
    {
        var lines = new List<string>();
        lines.AddRange(opCodeEmitter.Set(add.Left, Register.HL));
        lines.Add(opCodeEmitter.Set(Register.L, Register.A));
        lines.AddRange(opCodeEmitter.Set(add.Right, Register.HL));
        lines.Add(opCodeEmitter.Set(Register.L, Register.B));
        lines.Add(opCodeEmitter.Increment(Register.A, Register.B));
        lines.AddRange(opCodeEmitter.Set(Register.A, add.Target));
        return lines;
    }

    public IEnumerable<string> Call(SomeCompiler.Generation.Intermediate.Model.Codes.Call call)
    {
        // Only issue the call; stack cleanup is handled separately after capturing the return value.
        return new[] { opCodeEmitter.Call(call.Name) };
    }

    public IEnumerable<string> CleanArgs(SomeCompiler.Generation.Intermediate.Model.Codes.CleanArgs clean)
    {
        return opCodeEmitter.AdjustSPPreserveHL(clean.ArgCount * 2);
    }

    public IEnumerable<string> Divide(Divide divide)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> EmptyReturn()
    {
        // Use academic epilogue if this function has parameters
        if (parameterCount > 0)
            return opCodeEmitter.AcademicEpilogueAndReturn(frameSize);
        else if (frameSize > 0 || opCodeEmitter.HasAnyReferences())
            return opCodeEmitter.EpilogueAndReturn();
        else
            return new[] { "\tRET" };
    }

    public IEnumerable<string> Halt()
    {
        return new[]
        {
            opCodeEmitter.Halt()
        };
    }

    public IEnumerable<string> LoadHLImm(LoadHLImm op)
    {
        return new[] { opCodeEmitter.Set(op.Value, Register.HL) };
    }

    public IEnumerable<string> LoadHLRef(LoadHLRef op)
    {
        return opCodeEmitter.Set(op.From, Register.HL);
    }

    public IEnumerable<string> Assign(Assign assign)
    {
        var lines = new List<string>();
        lines.AddRange(opCodeEmitter.Set(assign.Source, Register.HL));
        if (opCodeEmitter.HasReference(assign.Target))
        {
            lines.AddRange(opCodeEmitter.Set(Register.HL, assign.Target));
        }
        return lines;
    }

    public IEnumerable<string> Return(Return ret)
    {
        var lines = new List<string>();
        // Only load from reference if it was materialized (exists in table)
        // If not materialized, assume the value is already in HL
        if (opCodeEmitter.HasReference(ret.Reference))
        {
            lines.AddRange(opCodeEmitter.Set(ret.Reference, Register.HL));
        }
        // Use academic epilogue if this function has parameters
        if (parameterCount > 0)
            lines.AddRange(opCodeEmitter.AcademicEpilogueAndReturn(frameSize));
        else if (frameSize > 0 || opCodeEmitter.HasAnyReferences())
            lines.AddRange(opCodeEmitter.EpilogueAndReturn());
        else
            lines.Add("\tRET");
        return lines;
    }

    public IEnumerable<string> Multiply(Multiply multiply)
    {
        var lines = new List<string>();
        // Load left into HL, then copy to DE (multiplicand)
        lines.AddRange(opCodeEmitter.Set(multiply.Left, Register.HL));
        lines.Add(opCodeEmitter.Set(Register.H, Register.D));
        lines.Add(opCodeEmitter.Set(Register.L, Register.E));
        // Load right into HL, then copy to BC (multiplier)
        lines.AddRange(opCodeEmitter.Set(multiply.Right, Register.HL));
        lines.Add(opCodeEmitter.Set(Register.H, Register.B));
        lines.Add(opCodeEmitter.Set(Register.L, Register.C));
        // Call multiply and store result from HL
        lines.Add(opCodeEmitter.Call("MUL16"));
        lines.AddRange(opCodeEmitter.Set(Register.HL, multiply.Target));
        return lines;
    }

    public IEnumerable<string> Param(Param param)
    {
        var lines = new List<string>();
        lines.AddRange(opCodeEmitter.Set(param.Argument, Register.HL));
        lines.Add(opCodeEmitter.Push(Register.HL));
        return lines;
    }

    public IEnumerable<string> ParamConst(ParamConst param)
    {
        return new[]
        {
            opCodeEmitter.Set(param.Value, Register.HL),
            opCodeEmitter.Push(Register.HL),
        };
    }

    public IEnumerable<string> AssignFromReturn(AssignFromReturn afr)
    {
        return opCodeEmitter.Set(Register.HL, afr.Target);
    }

    public IEnumerable<string> Subtraction(Subtract sub)
    {
        var lines = new List<string>();
        // right -> HL -> DE
        lines.AddRange(opCodeEmitter.Set(sub.Right, Register.HL));
        lines.Add(opCodeEmitter.Set(Register.H, Register.D));
        lines.Add(opCodeEmitter.Set(Register.L, Register.E));
        // left -> HL
        lines.AddRange(opCodeEmitter.Set(sub.Left, Register.HL));
        // HL = HL - DE (clear carry then SBC HL,DE)
        lines.Add(opCodeEmitter.XorA());
        lines.Add(opCodeEmitter.SbcHlDe());
        // store to target
        lines.AddRange(opCodeEmitter.Set(Register.HL, sub.Target));
        return lines;
    }

    public IEnumerable<string> Jump(Jump jump)
    {
        return new[] { opCodeEmitter.Jump(jump.Label) };
    }

    public IEnumerable<string> BranchIfZero(BranchIfZero brz)
    {
        var lines = new List<string>();
        lines.AddRange(opCodeEmitter.Set(brz.Condition, Register.HL));
        lines.AddRange(opCodeEmitter.BranchIfHLZero(brz.Label));
        return lines;
    }

    public IEnumerable<string> BranchIfNotZero(BranchIfNotZero brnz)
    {
        var lines = new List<string>();
        lines.AddRange(opCodeEmitter.Set(brnz.Condition, Register.HL));
        lines.AddRange(opCodeEmitter.BranchIfHLNotZero(brnz.Label));
        return lines;
    }
}
