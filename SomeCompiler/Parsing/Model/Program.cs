namespace SomeCompiler.Parsing.Model;

public class Program
{
    public Program(Functions functions)
    {
        Functions = functions;
    }

    public Functions Functions { get; }

    public override string ToString() => Functions.ToString();
}