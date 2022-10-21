namespace SomeCompiler.VirtualMachine.Commands;

internal class HaltCommand : Command
{
    public IMachine Machine { get; }

    public HaltCommand(IMachine machine)
    {
        Machine = machine;
    }

    public override void Execute()
    {
        Machine.Halt();
    }
}