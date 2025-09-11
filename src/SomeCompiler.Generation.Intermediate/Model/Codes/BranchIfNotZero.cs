namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record BranchIfNotZero(CodeGeneration.Model.Classes.Reference Condition, string Label) : Code
{
    public override IEnumerable<CodeGeneration.Model.Classes.Reference> GetReferences()
    {
        yield return Condition;
    }

    public override string ToString(Dictionary<CodeGeneration.Model.Classes.Reference, string> map)
    {
        return $"brnz {map[Condition]}, {Label}";
    }
}

