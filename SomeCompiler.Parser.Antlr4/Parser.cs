using Antlr4.Runtime;
using CSharpFunctionalExtensions;
using SomeCompiler.Parser.Model;

namespace SomeCompiler.Parser.Antlr4;

public class Parser
{
    public Result<Program> Parse(string input)
    {
        var lexer = new CLexer(CharStreams.fromString(input));
        var parser = new CParser(new CommonTokenStream(lexer));
        var p = parser.translationUnit();
        return Parse(p);
    }

    private Result<Program> Parse(CParser.TranslationUnitContext input)
    {
        var functions = GetFunctions(input).ToList();

        return new Program(new Functions(functions));
    }

    private IEnumerable<Function> GetFunctions(CParser.TranslationUnitContext input)
    {
        foreach (var externDecl in input.children.OfType<CParser.ExternalDeclarationContext>())
        {
            foreach (var c in externDecl.children)
            {
                if (c is CParser.FunctionDefinitionContext def)
                {
                    yield return new Function(def.GetChild(1).GetChild(0).GetChild(0).GetText(), new ArgumentList(), new Block());
                }
            }
        }
    }
}