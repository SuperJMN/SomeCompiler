namespace RetroSharp.Generation.Intermediate.Model;

internal class ConstantReference : CodeGeneration.Model.Classes.Reference
{
    public ConstantReference(int constant)
    {
        Constant = constant;
    }

    public int Constant { get; }
}
