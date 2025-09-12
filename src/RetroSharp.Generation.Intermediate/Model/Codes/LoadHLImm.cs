namespace RetroSharp.Generation.Intermediate.Model.Codes;

public record LoadHLImm(int Value) : Code
{
    public override string ToString(Dictionary<CodeGeneration.Model.Classes.Reference, string> map)
    {
        return $"HL = {Value}";
    }
}
