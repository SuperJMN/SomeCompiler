namespace RetroSharp.Generation.Intermediate.Model.Codes;

public record Jump(string Label) : Code
{
    public override string ToString(Dictionary<CodeGeneration.Model.Classes.Reference, string> map)
    {
        return $"jump {Label}";
    }
}

