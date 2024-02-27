using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model;

public class LabelReference : Reference
{
    public LabelReference(string label)
    {
        Label = label;
    }

    public string Label { get; }
}