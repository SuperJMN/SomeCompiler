namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public abstract record Code
{
    public virtual IEnumerable<CodeGeneration.Model.Classes.Reference> GetReferences() => Enumerable.Empty<CodeGeneration.Model.Classes.Reference>();

    public abstract string ToString(Dictionary<CodeGeneration.Model.Classes.Reference, string> map);
}
