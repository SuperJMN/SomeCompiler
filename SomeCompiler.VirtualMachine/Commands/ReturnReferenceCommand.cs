using CodeGeneration.Model.Classes;

namespace SomeCompiler.VirtualMachine.Commands;

internal class ReturnReferenceCommand : Command
{
    public IMachine Machine { get; }
    public Reference Reference { get; }

    public ReturnReferenceCommand(IMachine machine, Reference reference)
    {
        Machine = machine;
        Reference = reference;
    }

    public override void Execute()
    {
        var previousInstruction = Machine.Stack.Pop();
        Machine.Stack.Push(Machine.Memory[Machine.Variables[Reference]]);
        Machine.ExecutionPointer = Machine.Memory.ToList().IndexOf(previousInstruction);
    }
}