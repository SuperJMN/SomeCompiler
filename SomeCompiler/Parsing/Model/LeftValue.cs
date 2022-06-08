namespace SomeCompiler.Parsing.Model;

public class LeftValue
{
    public LeftValue(string identifier)
    {
        Identifier = identifier;
    }

    public string Identifier { get; }

    public override string ToString() => Identifier;
}