namespace SomeCompiler.Generation.Intermediate.Model;

public class LabelReference : CodeGeneration.Model.Classes.Reference
{
    public LabelReference(string label)
    {
        Label = label;
    }

    public string Label { get; }
}
