using CSharpFunctionalExtensions;

namespace SomeCompiler.VirtualMachine.Commands;

internal class CallCommand : Command
{
    public IMachine Machine { get; }
    public string CallName { get; }

    public CallCommand(IMachine machine, string callName)
    {
        Machine = machine;
        CallName = callName;
    }

    public override void Execute()
    {
        var instructionMemoryEntries = from ins in Machine.Memory.OfType<InstructionMemoryEntry>()
                                       from l in ins.Label.ToList()
                                       where l.Name == CallName
                                       select ins;

        Machine.Stack.Push(Machine.Memory[Machine.ExecutionPointer + 1]);
        Machine.ExecutionPointer = Machine.Memory.ToList().IndexOf(instructionMemoryEntries.First());
    }
}