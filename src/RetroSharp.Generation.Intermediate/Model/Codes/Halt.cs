namespace RetroSharp.Generation.Intermediate.Model.Codes;

public record Halt : Code
{
    public override string ToString(Dictionary<CodeGeneration.Model.Classes.Reference, string> map)
    {
        return "halt";
    }
}
