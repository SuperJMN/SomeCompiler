using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record And(Reference Target, Reference Left, Reference Right) : Code
{
    public override IEnumerable<Reference> GetReferences()
    {
        yield return Target;
        yield return Left;
        yield return Right;
    }

    public override string ToString(Dictionary<Reference, string> map)
    {
        return $"{map[Target]} = {map[Left]} && {map[Right]}";
    }
}