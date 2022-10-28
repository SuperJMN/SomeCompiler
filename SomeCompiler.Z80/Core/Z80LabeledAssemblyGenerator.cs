using CSharpFunctionalExtensions;
using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Z80.Core;

public class Z80LabeledAssemblyGenerator
{
    private readonly Z80IntermediateToOpCodeEmitter z80IntermediateToOpCodeEmitter;

    public Z80LabeledAssemblyGenerator(Z80IntermediateToOpCodeEmitter z80IntermediateToOpCodeEmitter)
    {
        this.z80IntermediateToOpCodeEmitter = z80IntermediateToOpCodeEmitter;
    }

    public string Generate(LabeledInstruction labeledInstruction)
    {
        var intrs = Generate(labeledInstruction.Code);
        var labelStr = labeledInstruction.Label.Match(x => x.Name, () => "");
        var instrs = string.Join(Environment.NewLine, intrs);
        return labelStr + instrs;
    }

    private IEnumerable<string> Generate(Code code)
    {
        return code switch
        {
            Add add => z80IntermediateToOpCodeEmitter.Addition(add),
            Assign assign => z80IntermediateToOpCodeEmitter.Assign(assign),
            AssignConstant assignConstant => z80IntermediateToOpCodeEmitter.AssignConstant(assignConstant),
            Call call => z80IntermediateToOpCodeEmitter.Call(call),
            Divide divide => z80IntermediateToOpCodeEmitter.Divide(divide),
            EmptyReturn emptyReturn => z80IntermediateToOpCodeEmitter.EmptyReturn(),
            Halt halt => z80IntermediateToOpCodeEmitter.Halt(),
            Multiply multiply => throw new NotImplementedException(),
            Return @return => throw new NotImplementedException(),
            Subtract subtract => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(code))
        };
    }
}