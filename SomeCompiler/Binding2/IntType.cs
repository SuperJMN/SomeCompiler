using CSharpFunctionalExtensions;
using SomeCompiler.Parser.Model;

namespace SomeCompiler.Binding2;

public record IntType : SymbolType
{
    public static readonly IntType Instance = new();
}

public class SemanticAnalyzer
{
    public (SemanticNode, List<string>) AnalyzeNode(INode node, Scope scope)
    {
        return node switch
        {
            DeclarationStatement varDecl => DeclareSymbol(varDecl, scope),
            Block block => AnalyzeBlock(block, scope),
            _ => throw new NotImplementedException(),
        };
    }
    private (SemanticNode, List<string>) AnalyzeBlock(Block block, Scope scope)
    {
        var errors = new List<string>();
        var semanticStatements = new List<SemanticNode>();
        foreach (var stmt in block)
        {
            (var semanticNode, var newErrors) = AnalyzeNode(stmt, scope);
            semanticStatements.Add(semanticNode);
            errors.AddRange(newErrors);
        }
        return (new SemanticBlock(semanticStatements, scope), errors);
    }
    private (SemanticNode, List<string>) DeclareSymbol(DeclarationStatement decl, Scope scope)
    {
        var newScope = scope.TryDeclare(decl.Name, IntType.Instance)
            .Match(
                newScope => ((SemanticNode)new SemanticVarDecl(decl.Name, IntType.Instance, newScope), new List<string>()),
                s => (new SemanticError(s), [s]));
        return newScope;
    }
}

public class SemanticError : SemanticNode
{
    public string Message { get; }
    public SemanticError(string message)
    {
        Message = message;
    }
}