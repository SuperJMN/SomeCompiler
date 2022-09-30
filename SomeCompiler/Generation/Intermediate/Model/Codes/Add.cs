using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record Add(Reference Target, Reference Left, Reference Right) : Code;