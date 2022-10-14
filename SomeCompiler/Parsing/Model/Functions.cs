namespace SomeCompiler.Parsing.Model;

public class Functions : List<Function>, INode
{
    public Functions(Functions functions) : base(functions)
    {
    }

    public Functions()
    {
    }

    public IEnumerable<INode> Children => this;
}