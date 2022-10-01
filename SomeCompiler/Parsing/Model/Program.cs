namespace SomeCompiler.Parsing.Model;

public record Program(Functions Functions)
{
    public override string ToString()
    {
        return Functions.ToString();
    }
}