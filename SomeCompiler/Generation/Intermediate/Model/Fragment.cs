using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model;

public class Fragment
{
    public Fragment(Func<Reference, Code> codeFactory) : this(codeFactory, Enumerable.Empty<Code>())
    {
    }

    public Fragment(Func<Reference, Code> codeFactory, IEnumerable<Code> codes)
    {
        Reference = new Placeholder();
        Codes = codes.Concat(new[] {codeFactory(Reference)});
    }

    public Fragment(Reference reference, IEnumerable<Code> codes)
    {
        Reference = reference;
        Codes = codes;
    }

    public Reference Reference { get; }
    public IEnumerable<Code> Codes { get; }

    public Fragment Prepend(Fragment fragment)
    {
        return new Fragment(Reference, fragment.Codes.Concat(Codes));
    }
}