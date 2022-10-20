namespace SomeCompiler.VirtualMachine;

public class VirtualMachineState
{
    public long ExecutionPointer { get; }
    public IReadOnlyList<MemoryEntry> StackContents { get; }
    public MemoryStore Memory { get; }

    public VirtualMachineState(long executionPointer, IReadOnlyList<MemoryEntry> stackContents, IReadOnlyList<MemoryEntry> memory, IDictionary<string, int> mapping)
    {
        ExecutionPointer = executionPointer;
        StackContents = stackContents;
        Memory = new MemoryStore(memory, mapping);
    }
}

public class MemoryStore
{
    public IReadOnlyList<MemoryEntry> Memory { get; }
    public IDictionary<string, int> Mapping { get; }

    public MemoryStore(IReadOnlyList<MemoryEntry> memory, IDictionary<string, int> mapping)
    {
        Memory = memory;
        Mapping = mapping;
    }

    public DataMemoryEntry this[string variableName]
    {
        get
        {
            var memoryEntry = Mapping[variableName];
            return (DataMemoryEntry) Memory[memoryEntry];
        }
    }
}