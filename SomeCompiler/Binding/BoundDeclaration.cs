using SomeCompiler.Binding.Model;
using SomeCompiler.Parser.Model;

namespace SomeCompiler.Binding;

public record BoundDeclaration : BoundStatement
{
    public BoundDeclaration(ArgumentType ArgumentType, string Name)
    {
        this.ArgumentType = ArgumentType;
        this.Name = Name;
    }

    public ArgumentType ArgumentType { get; init; }
    public string Name { get; init; }

    public void Deconstruct(out ArgumentType ArgumentType, out string Name)
    {
        ArgumentType = this.ArgumentType;
        Name = this.Name;
    }

    public override string ToString()
    {
        return $"{ArgumentType} {Name};";
    }
}