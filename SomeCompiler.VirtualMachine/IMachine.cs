using CodeGeneration.Model.Classes;

namespace SomeCompiler.VirtualMachine;

internal interface IMachine
{
    void Halt();
    IList<MemoryEntry> Memory { get; }
    int ExecutionPointer { get; set; }
    Stack<MemoryEntry> Stack { get; }
    Dictionary<Reference, int> Variables { get; }
}