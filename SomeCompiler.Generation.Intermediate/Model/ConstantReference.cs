using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model;

internal class ConstantReference : Reference
{
    public ConstantReference(int constant)
    {
        Constant = constant;
    }

    public int Constant { get; }
}