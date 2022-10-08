namespace SomeCompiler.Parsing.Model;

public class Functions : List<Function>
{
    public Functions(IEnumerable<Function> statements) : base(statements)
    {
    }

    public Functions(params Function[] statements) : base(statements)
    {
    }

    public override string ToString()
    {
        return this.JoinWithLines();
    }
}