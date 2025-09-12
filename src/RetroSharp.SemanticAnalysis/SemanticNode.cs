namespace RetroSharp.SemanticAnalysis;

public abstract class SemanticNode
{
    public abstract void Accept(INodeVisitor visitor);
    public IEnumerable<string> Errors { get; init; } = new List<string>();
    public IEnumerable<string> AllErrors => this.GetAllErrors();
    public abstract IEnumerable<SemanticNode> Children { get; }
}