namespace SomeCompiler.SemanticAnalysis;

public class FunctionCallExpressionNode : ExpressionNode
{
    public string Name { get; }
    public IReadOnlyList<ExpressionNode> Arguments { get; }

    public FunctionCallExpressionNode(string name, IReadOnlyList<ExpressionNode> arguments)
    {
        Name = name;
        Arguments = arguments;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitFunctionCall(this);
    }

    public override IEnumerable<SemanticNode> Children => Arguments;
}

