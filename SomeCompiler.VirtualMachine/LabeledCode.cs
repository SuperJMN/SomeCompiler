using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.VirtualMachine;

public record LabeledCode(Maybe<Label> Label, Code Code);