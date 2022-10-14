namespace SomeCompiler.Parsing.Model;

public record Function(string Name, ArgumentList ArgumentList, CompoundStatement CompoundStatement) : INode
{
    public IEnumerable<INode> Children => CompoundStatement.Children;
}