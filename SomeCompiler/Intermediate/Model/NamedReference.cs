using CodeGeneration.Model.Classes;

namespace SomeCompiler.Intermediate.Model;

public class NamedReference : Reference
{
    public NamedReference(string value)
    {
        Value = value;
    }

    public string Value { get; }
}