namespace RetroSharp.Generation.Intermediate.Model.Codes;

public record LocalLabel(string Name) : Code
{
    public override string ToString(Dictionary<CodeGeneration.Model.Classes.Reference, string> map)
    {
        return $"label {Name}";
    }
}

