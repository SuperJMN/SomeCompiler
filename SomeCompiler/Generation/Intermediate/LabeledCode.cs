using CSharpFunctionalExtensions;
using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Generation.Intermediate;

public record LabeledCode(Maybe<Label> Label, Code Code);