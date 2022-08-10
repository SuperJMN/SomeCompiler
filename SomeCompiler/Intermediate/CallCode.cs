namespace SomeCompiler.Intermediate;

internal class CallCode : IntermediateCode
{
    public string LabelName { get; }

    public CallCode(string labelName)
    {
        LabelName = labelName;
    }

    public override string ToString() => $"Call {LabelName}";
}