using Antlr4.Runtime.Tree;
using Zafiro.Core.Mixins;

namespace SomeCompiler.Parser.Antlr4;

public class ToStringVisitor : IParseTreeVisitor<string>
{
    private int indentLevel = 0;

    public string Visit(IParseTree tree)
    {
        return tree.Accept(this);
    }

    public string VisitChildren(IRuleNode node)
    {
        var rootName = new string('\t', indentLevel) + node.GetType().Name.Replace("Context", "");

        indentLevel++;
        var visitChildren = node.Children().Select(x => x.Accept(this)).Join(" ");
        indentLevel--;

        var text = "";
        if (node.ChildCount == 1 && node.GetChild(0) is ITerminalNode n)
        {
            text = n.GetText();
        }
        
        return rootName + "(" + text + "):\n" + visitChildren;
    }

    public string VisitTerminal(ITerminalNode node)
    {
        return "";
    }

    public string VisitErrorNode(IErrorNode node)
    {
        return node.Accept(this);
    }
}