namespace SomeCompiler.Intermediate;

internal class LabelCode : IntermediateCode
{
    public string Name { get; }

    public LabelCode(string name)
    {
        Name = name;
    }

    public override string ToString() => $"Label {Name}";
}