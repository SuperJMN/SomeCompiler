namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record ParamConst(int Value) : Code
{
    public override string ToString(Dictionary<CodeGeneration.Model.Classes.Reference, string> map)
    {
        return $"param {Value}";
    }
}
