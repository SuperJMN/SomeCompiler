namespace SomeCompiler.Z80;

public class GeneratedProgram
{
    public string Assembly { get; }

    public GeneratedProgram(string assembly, Dictionary<string, int> map)
    {
        Assembly = assembly;
    }
}