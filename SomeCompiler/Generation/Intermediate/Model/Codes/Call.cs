using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record Call(string Name) : Code
{
    public override string ToString()
    {
        return $"{base.ToString()}";
    }

    public override string ToString(Dictionary<Reference, string> map)
    {
        return $"call {Name}";
    }
}