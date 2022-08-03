// See https://aka.ms/new-console-template for more information

using SomeCompiler.Parsing;

Console.WriteLine("Hello, World!");

SomeGrammar grammar = new();
var content = grammar.ToGrammarFileContent();
var str = string.Join(Environment.NewLine, content);
Console.WriteLine(str);