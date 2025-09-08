namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public record EmptyReturn : Code
{
    public override string ToString(Dictionary<CodeGeneration.Model.Classes.Reference, string> map)
    {
        return "return";
    }
}
