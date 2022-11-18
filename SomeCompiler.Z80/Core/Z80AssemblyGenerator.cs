using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Z80.Core;

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
            Subtract subtract => throw new NotImplementedException(),
            Label label => new[] { $"{label.Name}:"},
            _ => throw new ArgumentOutOfRangeException(nameof(code))
        };
    }
}