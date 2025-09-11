namespace SomeCompiler.Generation.Intermediate.Model;

using ModelCode = SomeCompiler.Generation.Intermediate.Model.Codes.Code;
using Ref = CodeGeneration.Model.Classes.Reference;

public class Fragment
{
    public Fragment(Func<Ref, ModelCode> codeFactory) : this(codeFactory, Enumerable.Empty<ModelCode>())
    {
    }

    public Fragment(Func<Ref, ModelCode> codeFactory, IEnumerable<ModelCode> codes)
    {
        Reference = new Placeholder();
        Codes = codes.Concat(new[] { codeFactory(Reference) });
    }

    public Fragment(Ref reference, IEnumerable<ModelCode> codes)
    {
        Reference = reference;
        Codes = codes;
    }

    public Ref Reference { get; }
    public IEnumerable<ModelCode> Codes { get; }

    public Fragment Prepend(Fragment fragment)
    {
        return new Fragment(Reference, fragment.Codes.Concat(Codes));
    }
}
