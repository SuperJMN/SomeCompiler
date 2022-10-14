using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record Assign(Reference Target, Reference Source) : Code
{
    public override IEnumerable<Reference> GetReferences()
    {
        yield return Target;
        yield return Source;
    }

    public override string ToString(Dictionary<Reference, string> map)
    {
        return $"{map[Target]} = {map[Source]}";
    }
}