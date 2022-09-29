using CodeGeneration.Model.Classes;

namespace SomeCompiler.Intermediate.Model;

public record Code(Reference Destination, Reference Left, Reference? Right, Operator Operator);