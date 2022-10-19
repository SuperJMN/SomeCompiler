namespace SomeCompiler.VirtualMachine;

public class VirtualMachineState
{
    public long ExecutionPointer { get; }
    public IReadOnlyList<MemoryEntry> StackContents { get; }
    public IReadOnlyList<MemoryEntry> Memory { get; }

    public VirtualMachineState(long executionPointer, IReadOnlyList<MemoryEntry> stackContents, IReadOnlyList<MemoryEntry> memory)
    {
        ExecutionPointer = executionPointer;
        StackContents = stackContents;
        Memory = memory;
    }
}