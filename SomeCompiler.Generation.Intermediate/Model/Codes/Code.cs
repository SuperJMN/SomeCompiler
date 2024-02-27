using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public abstract record Code
{
    public virtual IEnumerable<Reference> GetReferences() => Enumerable.Empty<Reference>();

    public abstract string ToString(Dictionary<Reference, string> map);
}