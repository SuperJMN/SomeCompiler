using SomeCompiler.Parsing;

namespace SomeCompiler.Compilation.Model;

public class BoundFunction
{
    public BoundFunction(ReturnType returnType, string name, BoundBlock block)
    {
        ReturnType = returnType;
        Name = name;
        Block = block;
    }

    public ReturnType ReturnType { get; }
    public string Name { get; }
    public BoundBlock Block { get; }

    public override string ToString() => new object[]{ $"{ReturnType} {Name}()", Block }.JoinWithLines();
}