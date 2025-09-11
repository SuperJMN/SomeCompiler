namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record Call(string Name, int ArgCount) : Code
{
    public override string ToString(Dictionary<CodeGeneration.Model.Classes.Reference, string> map)
    {
        return $"call {Name} ({ArgCount})";
    }
}
