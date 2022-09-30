using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record Multiply(Reference Target, Reference Left, Reference Right) : Code;