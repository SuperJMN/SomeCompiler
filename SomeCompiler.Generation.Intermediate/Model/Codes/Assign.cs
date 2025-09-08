namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record Assign(CodeGeneration.Model.Classes.Reference Target, CodeGeneration.Model.Classes.Reference Source) : Code
{
    public override IEnumerable<CodeGeneration.Model.Classes.Reference> GetReferences()
    {
        yield return Target;
        yield return Source;
    }

    public override string ToString(Dictionary<CodeGeneration.Model.Classes.Reference, string> map)
    {
        return $"{map[Target]} = {map[Source]}";
    }
}
