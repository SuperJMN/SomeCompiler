using CodeGeneration.Model.Classes;

namespace SomeCompiler.VirtualMachine.Commands;

internal class OrCommand : Command
{
    public OrCommand(IMachine machine, Reference left, Reference right, Reference destination)
    {
        Machine = machine;
        Left = left;
        Right = right;
        Destination = destination;
    }

    public IMachine Machine { get; }
    public Reference Left { get; }
    public Reference Right { get; }
    public Reference Destination { get; }

    public override void Execute()
    {
        var leftValue = ((DataMemoryEntry)Machine.Memory[Machine.Variables[Left]]).Value;
        var rightValue = ((DataMemoryEntry)Machine.Memory[Machine.Variables[Right]]).Value;
        Machine.Memory[Machine.Variables[Destination]] = new DataMemoryEntry(Convert.ToInt32(Convert.ToBoolean(leftValue) || Convert.ToBoolean(rightValue)));
        Machine.ExecutionPointer++;
    }
}