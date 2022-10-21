namespace SomeCompiler.VirtualMachine.Commands;

internal class ReturnCommand : Command
{
    public IMachine Machine { get; }

    public ReturnCommand(IMachine machine)
    {
        Machine = machine;
    }

    public override void Execute()
    {
        var previousInstruction = Machine.Stack.Pop();
        Machine.ExecutionPointer = Machine.Memory.ToList().IndexOf(previousInstruction);
    }
}