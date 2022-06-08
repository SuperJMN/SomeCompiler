using EasyParse.Parsing;
using SomeCompiler.Parsing.Model;

namespace SomeCompiler.Parsing;

public class SomeParser
{
    public CompilationResult<Program> Parse(string source)
    {
        var arithmeticGrammar = new SomeGrammar();
        var parser = arithmeticGrammar.BuildCompiler<Program>();
        return parser.Compile(source);
    }
}