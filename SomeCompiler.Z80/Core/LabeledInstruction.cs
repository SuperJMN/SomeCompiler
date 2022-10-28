using CSharpFunctionalExtensions;
using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Z80.Core;

public record LabeledInstruction(Maybe<Label> Label, Code Code);