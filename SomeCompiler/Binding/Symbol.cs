using SomeCompiler.Binding.Model;

namespace SomeCompiler.Binding;

public class Symbol
{
    public Symbol(BoundType type)
    {
        Type = type;
    }

    public BoundType Type { get; set; }
}
