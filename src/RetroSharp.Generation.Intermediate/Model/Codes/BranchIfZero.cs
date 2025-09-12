namespace RetroSharp.Generation.Intermediate.Model.Codes;

public record BranchIfZero(CodeGeneration.Model.Classes.Reference Condition, string Label) : Code
{
    public override IEnumerable<CodeGeneration.Model.Classes.Reference> GetReferences()
    {
        yield return Condition;
    }

    public override string ToString(Dictionary<CodeGeneration.Model.Classes.Reference, string> map)
    {
        return $"brz {map[Condition]}, {Label}";
    }
}

