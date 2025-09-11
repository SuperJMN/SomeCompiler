namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record AssignFromReturn(CodeGeneration.Model.Classes.Reference Target) : Code
{
    public override IEnumerable<CodeGeneration.Model.Classes.Reference> GetReferences()
    {
        yield return Target;
    }

    public override string ToString(Dictionary<CodeGeneration.Model.Classes.Reference, string> map)
    {
        return $"{map[Target]} = <ret>";
    }
}

