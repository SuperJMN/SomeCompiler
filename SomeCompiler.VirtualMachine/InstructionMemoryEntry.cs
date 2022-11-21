using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.VirtualMachine;

public record InstructionMemoryEntry(Code Code, Maybe<Label> Label) : MemoryEntry;