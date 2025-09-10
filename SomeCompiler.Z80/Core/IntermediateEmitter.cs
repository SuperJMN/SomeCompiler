using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Z80.Core;

public class IntermediateEmitter
{
    private readonly OpCodeEmitter opCodeEmitter;

    public IntermediateEmitter(OpCodeEmitter opCodeEmitter)
    {
        this.opCodeEmitter = opCodeEmitter;
    }

    public IEnumerable<string> AssignConstant(AssignConstant assignConstant)
    {
        var lines = new List<string>();
        lines.Add(opCodeEmitter.Set(assignConstant.Source, Register.HL));
        lines.AddRange(opCodeEmitter.Set(Register.HL, assignConstant.Target));
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
        var lines = new List<string>();
        lines.Add(opCodeEmitter.Call(call.Name));
        lines.AddRange(opCodeEmitter.AdjustSP(call.ArgCount * 2));
        return lines;
    }

    public IEnumerable<string> Divide(Divide divide)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> EmptyReturn()
    {
        return opCodeEmitter.EpilogueAndReturn();
    }

    public IEnumerable<string> Halt()
    {
        return new[]
        {
            opCodeEmitter.Halt()
        };
    }

    public IEnumerable<string> Assign(Assign assign)
    {
        var lines = new List<string>();
        lines.AddRange(opCodeEmitter.Set(assign.Source, Register.HL));
        lines.AddRange(opCodeEmitter.Set(Register.HL, assign.Target));
        return lines;
    }

    public IEnumerable<string> Return(Return ret)
    {
        var lines = new List<string>();
        lines.AddRange(opCodeEmitter.Set(ret.Reference, Register.HL));
        lines.AddRange(opCodeEmitter.EpilogueAndReturn());
        return lines;
    }

    public IEnumerable<string> Multiply(Multiply multiply)
    {
        var lines = new List<string>();
        // Load left into HL, then copy to BC
        lines.AddRange(opCodeEmitter.Set(multiply.Left, Register.HL));
        lines.Add(opCodeEmitter.Set(Register.H, Register.B));
        lines.Add(opCodeEmitter.Set(Register.L, Register.C));
        // Load right into HL, then copy to DE
        lines.AddRange(opCodeEmitter.Set(multiply.Right, Register.HL));
        lines.Add(opCodeEmitter.Set(Register.H, Register.D));
        lines.Add(opCodeEmitter.Set(Register.L, Register.E));
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

    public IEnumerable<string> AssignFromReturn(AssignFromReturn afr)
    {
        return opCodeEmitter.Set(Register.HL, afr.Target);
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
