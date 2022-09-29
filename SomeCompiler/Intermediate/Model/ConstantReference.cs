using CodeGeneration.Model.Classes;

namespace SomeCompiler.Intermediate.Model;

internal class ConstantReference : Reference
{
    public ConstantReference(int constant)
    {
        Constant = constant;
    }

    public int Constant { get; }
}