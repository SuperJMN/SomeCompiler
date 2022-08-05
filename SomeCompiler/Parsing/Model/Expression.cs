namespace SomeCompiler.Parsing.Model;

public class Expression
{
    public int Value { get; }

    public Expression(int additive)
    {
        this.Value = additive;
    }

    public override string ToString() => Value.ToString();
}