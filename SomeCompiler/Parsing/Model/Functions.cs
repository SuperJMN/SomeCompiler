namespace SomeCompiler.Parsing.Model;

public class Functions : List<Function>
{
    public Functions(IEnumerable<Function> items) : base(items.ToList())
    {
    }

    public override string ToString() => string.Join(Environment.NewLine, this);
}