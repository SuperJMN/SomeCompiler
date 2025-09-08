namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record Return(CodeGeneration.Model.Classes.Reference Reference) : Code
{
    public override string ToString(Dictionary<CodeGeneration.Model.Classes.Reference, string> map)
    {
        return $"return {map[Reference]}";
    }
}
