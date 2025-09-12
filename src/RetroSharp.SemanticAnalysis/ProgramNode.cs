using Zafiro.Core.Mixins;

namespace RetroSharp.SemanticAnalysis;

public class ProgramNode : SemanticNode
{
    public List<FunctionNode> Functions { get; }

    public ProgramNode(List<FunctionNode> functions)
    {
        Functions = functions;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitProgramNode(this);
    }

    public override IEnumerable<SemanticNode> Children => Functions;

    public override string ToString()
    {
        return Functions.JoinWithLines();
    }
}