namespace RetroSharp.Generation.Intermediate.Model.Codes;

public record Param(CodeGeneration.Model.Classes.Reference Argument) : Code
{
    public override IEnumerable<CodeGeneration.Model.Classes.Reference> GetReferences()
    {
        yield return Argument;
    }

    public override string ToString(Dictionary<CodeGeneration.Model.Classes.Reference, string> map)
    {
        return $"param {map[Argument]}";
    }
}

