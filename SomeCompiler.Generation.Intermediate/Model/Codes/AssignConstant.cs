namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record AssignConstant(CodeGeneration.Model.Classes.Reference Target, int Source) : Code
{
    public override string ToString()
    {
        return $"{Target}={Source}";
    }

    public override string ToString(Dictionary<CodeGeneration.Model.Classes.Reference, string> map)
    {
        return $"{map[Target]} = {Source}";
    }

    public override IEnumerable<CodeGeneration.Model.Classes.Reference> GetReferences()
    {
        yield return Target;
    }
}
