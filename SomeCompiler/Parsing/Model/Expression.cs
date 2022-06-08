namespace SomeCompiler.Parsing.Model;

public class Expression
{
    private readonly int additive;

    public Expression(int additive)
    {
        this.additive = additive;
    }

    public override string ToString() => additive.ToString();
}