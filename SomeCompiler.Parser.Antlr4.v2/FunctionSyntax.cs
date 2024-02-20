namespace SomeCompiler.Parser;

public class FunctionSyntax : Syntax
{
    public string Name { get; }
    public BlockSyntax Block { get; }
    public string Type { get; }

    public FunctionSyntax(string type, string name, BlockSyntax block)
    {
        Type = type;
        Name = name;
        Block = block;
    }

    public override void Accept(ISyntaxVisitor visitor)
    {
        visitor.VisitFunction(this);
    }
}