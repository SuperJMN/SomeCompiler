﻿namespace SomeCompiler.VirtualMachine;

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