﻿using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Binding2;

public record Symbol(string Name, SymbolType Type)
{
    public override string ToString() => Type + " " + Name;
}