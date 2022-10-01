using SomeCompiler.Generation.Intermediate;
using SomeCompiler.Parsing.Model;

namespace SomeCompiler.Compilation.Model;

public class BoundFunction
{
    public BoundFunction(ReturnType returnType, string name, BoundCompoundStatement compoundStatement)
    {
        ReturnType = returnType;
        Name = name;
        CompoundStatement = compoundStatement;
    }

    public ReturnType ReturnType { get; }
    public string Name { get; }
    public BoundCompoundStatement CompoundStatement { get; }

    public override string ToString() => new object[] { $"{ReturnType} {Name}()", CompoundStatement }.JoinWithLines();
}