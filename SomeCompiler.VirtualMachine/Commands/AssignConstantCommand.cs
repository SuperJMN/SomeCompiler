using CodeGeneration.Model.Classes;

namespace SomeCompiler.VirtualMachine.Commands;

internal class AssignConstantCommand : Command
{
    public IMachine Machine { get; }
    public int From { get; }
    public Reference To { get; }

    public AssignConstantCommand(IMachine machine, int from, Reference to)
    {
        Machine = machine;
        From = from;
        To = to;
    }

    public override void Execute()
    {
        Machine.Memory[Machine.Variables[To]] = new DataMemoryEntry(From);
        Machine.ExecutionPointer++;
    }
}