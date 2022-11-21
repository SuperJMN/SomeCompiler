namespace SomeCompiler.Compilation.Model;

public class FunctionDeclaration
{
    public FunctionDeclaration(string name)
    {
        Name = name;
    }

    public string Name { get; }
}