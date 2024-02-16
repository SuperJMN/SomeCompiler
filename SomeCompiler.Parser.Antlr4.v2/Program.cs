﻿// Template generated code from Antlr4BuildTasks.Template v 8.17

using Antlr4.Runtime;
using SomeCompiler.Generated;

namespace SomeCompiler.Parser.Antlr4.v2
{
    public class Program
    {
        static void Main(string[] args)
        {
            Try("void main(){ int a; a = 12; }");
        }

        static void Try(string input)
        {
            var str = new AntlrInputStream(input);
            System.Console.WriteLine(input);
            var lexer = new SomeLanguageLexer(str);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SomeLanguageParser(tokens);
            var listener_lexer = new ErrorListener<int>();
            var listener_parser = new ErrorListener<IToken>();
            lexer.AddErrorListener(listener_lexer);
            parser.AddErrorListener(listener_parser);
            var tree = parser.programa();
            if (listener_lexer.had_error || listener_parser.had_error)
                System.Console.WriteLine("error in parse.");
            else
                System.Console.WriteLine("parse completed.");
        }

        static string ReadAllInput(string fn)
        {
            var input = System.IO.File.ReadAllText(fn);
            return input;
        }
    }
}
