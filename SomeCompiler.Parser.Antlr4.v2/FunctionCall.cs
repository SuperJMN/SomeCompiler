namespace SomeCompiler.Parser;

public class FunctionCall : ExpressionSyntax
{
    public FunctionCall(string name)
    {
        Name = name;
    }

    public override void Accept(ISyntaxVisitor visitor)
    {
        visitor.VisitFunctionCall(this);
    }

    public string Name { get; }
}