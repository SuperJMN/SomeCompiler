namespace SomeCompiler.Compilation.Model;

public class CompiledProgram
{
    public IEnumerable<BoundFunction> Functions { get; }

    public CompiledProgram(IEnumerable<BoundFunction> functions)
    {
        Functions = functions;
    }

    public override string ToString() => Functions.JoinWithLines();
}