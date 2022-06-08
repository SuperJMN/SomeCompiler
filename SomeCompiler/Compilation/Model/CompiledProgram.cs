namespace SomeCompiler.Compilation.Model;

public class CompiledProgram
{
    public IEnumerable<FunctionDeclaration> Declarations { get; }

    public CompiledProgram(IEnumerable<FunctionDeclaration> declarations)
    {
        Declarations = declarations;
    }
}