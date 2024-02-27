using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record Label(string Name) : Code
{
    public override string ToString(Dictionary<Reference, string> map)
    {
        return $"label {Name}";
    }
}