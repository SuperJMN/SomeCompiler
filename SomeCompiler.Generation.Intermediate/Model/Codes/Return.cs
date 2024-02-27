using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record Return(Reference Reference) : Code
{
    public override string ToString(Dictionary<Reference, string> map)
    {
        return $"return {map[Reference]}";
    }
}