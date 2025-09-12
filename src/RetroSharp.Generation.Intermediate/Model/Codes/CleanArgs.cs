namespace RetroSharp.Generation.Intermediate.Model.Codes;

public record CleanArgs(int ArgCount) : Code
{
    public override string ToString(Dictionary<CodeGeneration.Model.Classes.Reference, string> map)
    {
        return $"clean_args {ArgCount}";
    }
}

