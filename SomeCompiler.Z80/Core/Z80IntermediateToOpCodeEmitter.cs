using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Z80.Core;

public class Z80IntermediateToOpCodeEmitter
{
    private readonly Z80OpCodeEmitter opCodeEmitter;

    public Z80IntermediateToOpCodeEmitter(Z80OpCodeEmitter opCodeEmitter)
    {
        this.opCodeEmitter = opCodeEmitter;
    }

    public IEnumerable<string> AssignConstant(AssignConstant assignConstant)
    {
        var str = new[]
        {
            opCodeEmitter.Set(assignConstant.Source, Register.Hl),
            opCodeEmitter.Set(Register.Hl, assignConstant.Target)
        };

        return str;
    }

    public string[] Addition(Add add)
    {
        return new[]
        {
            opCodeEmitter.Set(add.Left, Register.Hl),
            opCodeEmitter.Set(Register.L, Register.A),
            opCodeEmitter.Set(add.Right, Register.Hl),
            opCodeEmitter.Set(Register.L, Register.B),
            opCodeEmitter.Increment(Register.A, Register.B),
            opCodeEmitter.Set(Register.A, add.Target)
        };
    }

    public IEnumerable<string> Call(Call call)
    {
        return new[]
        {
            opCodeEmitter.Call(call.Name)
        };
    }

    public IEnumerable<string> Divide(Divide divide)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> EmptyReturn()
    {
        return new[]
        {
            opCodeEmitter.Return()
        };
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
        return new[]
        {
            opCodeEmitter.Set(assign.Source, Register.Hl),
            opCodeEmitter.Set(Register.Hl, assign.Target)
        };
    }

    public IEnumerable<string> Return(Return ret)
    {
        return new[]
        {
            opCodeEmitter.Set(ret.Reference, Register.Hl),
            opCodeEmitter.Return(),
        };
    }
}