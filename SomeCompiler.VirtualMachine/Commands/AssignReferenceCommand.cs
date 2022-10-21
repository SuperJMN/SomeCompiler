using CodeGeneration.Model.Classes;

namespace SomeCompiler.VirtualMachine.Commands;

internal class AssignReferenceCommand : Command
{
    public IMachine Machine { get; }
    public Reference From { get; }
    public Reference To { get; }

    public AssignReferenceCommand(IMachine machine, Reference from, Reference to)
    {
        Machine = machine;
        From = from;
        To = to;
    }

    public override void Execute()
    {
        Machine.Memory[Machine.Variables[To]] = Machine.Memory[Machine.Variables[From]];
        Machine.ExecutionPointer++;
    }
}