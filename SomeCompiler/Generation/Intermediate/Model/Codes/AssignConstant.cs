using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record AssignConstant(Reference Target, int Source) : Code
{
    public override string ToString()
    {
        return $"{Target}={Source}";
    }

    public override string ToString(Dictionary<Reference, string> map)
    {
        return $"{map[Target]} = {Source}";
    }

    public override IEnumerable<Reference> GetReferences()
    {
        yield return Target;
    }
}