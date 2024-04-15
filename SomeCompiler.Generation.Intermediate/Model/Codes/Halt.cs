using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record Halt : Code
{
    public override string ToString(Dictionary<Reference, string> map)
    {
        return "halt";
    }
}