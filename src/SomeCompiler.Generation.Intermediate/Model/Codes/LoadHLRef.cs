using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record LoadHLRef(CodeGeneration.Model.Classes.Reference From) : Code
{
    public override IEnumerable<CodeGeneration.Model.Classes.Reference> GetReferences()
    {
        yield return From;
    }

    public override string ToString(Dictionary<CodeGeneration.Model.Classes.Reference, string> map)
    {
        return $"HL = {map[From]}";
    }
}
