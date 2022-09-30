using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record Divide(Reference Target, Reference Left, Reference Right) : Code;