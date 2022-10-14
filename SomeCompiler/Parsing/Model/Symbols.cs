using System.Text.RegularExpressions;
using EasyParse.Fluent.Symbols;

namespace SomeCompiler.Parsing.Model;

public static class Symbols
{
    public static RegexSymbol IntKeyword => RegexSymbol.Create("int", new Regex(@"int"), s => s);
    public static RegexSymbol Identifier => RegexSymbol.Create("identifier", new Regex(@"\w+"), s => s);
    public static RegexSymbol EmptyBlock => RegexSymbol.Create("empty block", new Regex(@"{\s*}"), s => s);
}