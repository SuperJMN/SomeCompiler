using Zafiro.Core.Mixins;

namespace SomeCompiler.Binding.Model;

public class CompiledProgram
{
    public CompiledProgram(IEnumerable<BoundFunction> functions)
    {
        Functions = functions;
    }

    public IEnumerable<BoundFunction> Functions { get; }

    public override string ToString() => Functions.JoinWithLines();
}