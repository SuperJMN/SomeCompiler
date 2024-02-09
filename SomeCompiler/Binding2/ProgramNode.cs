﻿using Zafiro.Core.Mixins;

namespace SomeCompiler.Binding2;

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

    public override string ToString()
    {
        return Functions.JoinWithLines();
    }
}