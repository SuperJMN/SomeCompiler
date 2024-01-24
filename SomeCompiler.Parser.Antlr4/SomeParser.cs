using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CSharpFunctionalExtensions;
using SomeCompiler.Parser.Model;
using static CParser;

namespace SomeCompiler.Parser.Antlr4;

public class SomeParser
{
    private readonly ExpressionConverter converter = new ExpressionConverter();

    public Result<Program, List<string>> Parse(string input)
    {
        var lexer = new CLexer(CharStreams.fromString(input));
        var parser = new CParser(new CommonTokenStream(lexer));

        var p = parser.translationUnit();
        var b = new ToStringVisitor();
        var toString = p.Accept(b);
        return Parse(p);
    }

    private Result<Program, List<string>> Parse(CParser.TranslationUnitContext input)
    {
        var funcs = input.Descendants<CParser.FunctionDefinitionContext>().Select(ParseFunction);

        return new Program(new Functions(funcs.ToList()));
    }

    private Function ParseFunction(FunctionDefinitionContext func)
    {
        var returnType = new ReturnType(ParseReturnType(func.children[0]));
        var functionName = ParseFunctionName(func.children[1]);
        var args = ParseArgs(func.children[1]);
        var block = Maybe.From(func.compoundStatement()).Map(ParseBlock).GetValueOrDefault(new Block());
        return new Function(returnType, functionName, args, block);
    }

    private ParameterList ParseArgs(IParseTree funcChild)
    {
        return new ParameterList(funcChild.Descendants<ParameterDeclarationContext>().Select(ParseParameterDeclaration));
    }

    private Parameter ParseParameterDeclaration(ParameterDeclarationContext parameter)
    {
        var type = parameter.GetChild(0).GetText();
        var name = parameter.GetChild(1).GetText();
        return new Parameter(new ArgumentType(type), name);
    }

    private Statement ParseBlockItem(CParser.BlockItemContext arg)
    {
        var child = arg.GetChild(0);
        return child switch
        {
            CParser.DeclarationContext decl => ParseDeclaration(decl),
            CParser.StatementContext st => ParseStatement(st),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private DeclarationStatement ParseDeclaration(CParser.DeclarationContext decl)
    {
        var specifiers = decl.Descendant<CParser.DeclarationSpecifiersContext>();
        var type = specifiers.GetChild(0).GetText();
        string name;
        if (decl.GetChild(1) is InitDeclaratorListContext d)
        {
            var declaratorContext = d.GetChild(0) as InitDeclaratorContext;
            name = declaratorContext.GetChild(0).GetText();
            var value = declaratorContext.GetChild(2).GetText();
            return new DeclarationStatement(new ArgumentType(type), name, value);
        }

        name = decl.GetChild(0).GetChild(1).GetText();

        return new DeclarationStatement(new ArgumentType(type), name, Maybe<string>.None);
    }

    private Statement ParseStatement(CParser.StatementContext statementContext)
    {
        return statementContext.children[0] switch
        {
            CParser.ExpressionStatementContext expr => ParseExpressionStatement(expr),
            CParser.JumpStatementContext jmpStmt => new ReturnStatement(jmpStmt.ChildCount == 3 ? converter.ParseExpression((CParser.ExpressionContext)jmpStmt.GetChild(1)) : Maybe<Expression>.None),
            SelectionStatementContext sel => ParseSelectionStatement(sel),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private Statement ParseSelectionStatement(SelectionStatementContext sel)
    {
        // Parse the condition expression
        var condition = converter.ParseExpression(sel.expression());
        // Parse the "then" statement
        var thenBlock = ParseBlock(sel.statement()[0].compoundStatement());
        // Check if there's an "else" statement
        if (sel.Else() != null)
        {
            var elseBlock = ParseBlock(sel.statement(1).compoundStatement());
            return new IfElseStatement(condition, thenBlock, elseBlock);
        }
        else
        {
            return new IfElseStatement(condition, thenBlock, Maybe<Block>.None);
        }
    }

    private Block ParseBlock(CompoundStatementContext compoundStatement)
    {
        return Maybe.From(compoundStatement.blockItemList())
            .Map(i => new Block(i.children.Cast<BlockItemContext>().Select(ParseBlockItem)))
            .GetValueOrDefault([]);
    }

    private Statement ParseExpressionStatement(CParser.ExpressionStatementContext expressionStatement)
    {
        var expr = expressionStatement.children[0];
        return new ExpressionStatement(converter.ParseExpression((CParser.ExpressionContext)expr));
    }

    private static string ParseFunctionName(IParseTree child)
    {
        return child.Descendant<CParser.DirectDeclaratorContext>().children[0].GetText();
    }

    private static string ParseReturnType(IParseTree parseTree)
    {
        return parseTree.Descendant<CParser.TypeSpecifierContext>().children[0].GetText();
    }
}