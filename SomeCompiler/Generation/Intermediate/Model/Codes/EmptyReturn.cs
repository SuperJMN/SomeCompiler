using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record EmptyReturn : Code
{
    public override string ToString(Dictionary<Reference, string> map)
    {
        return "return";
    }
}