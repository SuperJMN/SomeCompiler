﻿namespace SomeCompiler.Parser;

public interface ISyntax
{
    public void Accept(ISyntaxVisitor visitor);
}