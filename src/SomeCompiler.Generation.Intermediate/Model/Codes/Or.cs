namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record Or(CodeGeneration.Model.Classes.Reference Target, CodeGeneration.Model.Classes.Reference Left, CodeGeneration.Model.Classes.Reference Right) : Code
{
    public override IEnumerable<CodeGeneration.Model.Classes.Reference> GetReferences()
    {
        yield return Target;
        yield return Left;
        yield return Right;
    }

    public override string ToString(Dictionary<CodeGeneration.Model.Classes.Reference, string> map)
    {
        return $"{map[Target]} = {map[Left]} || {map[Right]}";
    }
}
