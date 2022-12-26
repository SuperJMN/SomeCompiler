using System.ComponentModel;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CSharpFunctionalExtensions;
using SomeCompiler.Parser.Model;

namespace SomeCompiler.Parser.Antlr4;

public class Parser
{
    private ExpressionConverter converter;

    public Result<Program> Parse(string input)
    {
        var lexer = new CLexer(CharStreams.fromString(input));
        var parser = new CParser(new CommonTokenStream(lexer));

        var p = parser.translationUnit();
        var b = new ToStringVisitor();
        var toString = p.Accept(b);
        converter = new ExpressionConverter();
        return Parse(p);
        
    }

    private Result<Program> Parse(CParser.TranslationUnitContext input)
    {
        var funcs = input.Descendants<CParser.FunctionDefinitionContext>().Select(ParseFunction);
        
        return new Program(new Functions(funcs.ToList()));
    }

    private Function ParseFunction(CParser.FunctionDefinitionContext func)
    {
        var returnType = new ReturnType(ParseReturnType(func.children[0]));
        var functionName = ParseFunctionName(func.children[1]);
        var args = ParseArgs(func.children[1]);
        var block = ParseBlock(func);
        return new Function(returnType, functionName, args, block);
    }

    private ParameterList ParseArgs(IParseTree funcChild)
    {
        return new ParameterList(funcChild.Descendants<CParser.ParameterDeclarationContext>().Select(ParseParameterDeclaration));
    }

    private Parameter ParseParameterDeclaration(CParser.ParameterDeclarationContext parameter)
    {
        var type = parameter.GetChild(0).GetText();
        var name = parameter.GetChild(1).GetText();
        return new Parameter(new ArgumentType(type), name);
    }

    private Block ParseBlock(CParser.FunctionDefinitionContext functionDefinitionContext)
    {
        var cs = functionDefinitionContext.Descendant<CParser.CompoundStatementContext>();
        var statementContexts = cs.Descendants<CParser.StatementContext>();
        var statements = statementContexts.Select(ParseStatement);
        return new Block(statements);
    }

    private Statement ParseStatement(CParser.StatementContext statementContext)
    {
        return statementContext.children[0] switch
        {
            CParser.ExpressionStatementContext expr => ParseExpressionStatement(expr),
            CParser.JumpStatementContext jmpStmt => new ReturnStatement(jmpStmt.ChildCount == 3 ? converter.ParseExpression((CParser.ExpressionContext) jmpStmt.GetChild(1)) : Maybe<Expression>.None),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private Statement ParseExpressionStatement(CParser.ExpressionStatementContext expressionStatement)
    {
        var expr = expressionStatement.children[0];
        return new ExpressionStatement(converter.ParseExpression((CParser.ExpressionContext) expr));
    }

    private Expression ParseAssignmentExpression(CParser.AssignmentExpressionContext ase)
    {
        var leftValue = ParseLValue(ase.GetChild(0));
        var expression = converter.ParseExpression((CParser.ExpressionContext) ase.GetChild(2));

        return new AssignmentExpression(leftValue, expression);
    }

    private static LeftValue ParseLValue(IParseTree last)
    {
        return new LeftValue(last.GetText());
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