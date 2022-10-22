using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model.Codes;

public abstract record Code
{
    public virtual IEnumerable<Reference> GetReferences() => Enumerable.Empty<Reference>();

    public static Code Halt() => new Halt();
    public static Code Return(Reference? reference = null) => reference is not null ? new Return(reference) : new EmptyReturn();
    public static Code Add(Reference target, Reference left, Reference right) => new Add(target, left, right);
    public static Code Multiply(Reference target, Reference left, Reference right) => new Multiply(target, left, right);
    public static Code Subtract(Reference target, Reference left, Reference right) => new Subtract(target, left, right);
    public static Code Divide(Reference target, Reference left, Reference right) => new Divide(target, left, right);
    public static Code Label(string name) => new Label(name);
    public static Code Call(string name) => new Call(name);

    public abstract string ToString(Dictionary<Reference, string> map);
}