namespace SomeCompiler.VirtualMachine;

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