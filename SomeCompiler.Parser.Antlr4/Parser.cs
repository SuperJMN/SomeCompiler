using System.ComponentModel;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
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
        var b = new ToStringVisitor();
        var toString = p.Accept(b);
        return Parse(p);
    }

    private Result<Program> Parse(CParser.TranslationUnitContext input)
    {
        var funcs = input.Descendants<CParser.FunctionDefinitionContext>().Select(ParseFunction);
        
        return (Program) new Program(new Functions(funcs.ToList()));
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

    private static Block ParseBlock(CParser.FunctionDefinitionContext functionDefinitionContext)
    {
        var cs = functionDefinitionContext.Descendant<CParser.CompoundStatementContext>();
        var statementContexts = cs.Descendants<CParser.StatementContext>();
        var statements = statementContexts.Select(ParseStatement);
        return new Block(statements);
    }

    private static Statement ParseStatement(CParser.StatementContext statementContext)
    {
        return statementContext.children[0] switch
        {
            CParser.ExpressionStatementContext expr => ParseExpressionStatement(expr),
            CParser.JumpStatementContext jmpStmt => new ReturnStatement(ParseExpression(jmpStmt.GetChild(1))),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static Statement ParseExpressionStatement(CParser.ExpressionStatementContext expr)
    {
        var assignmentExpression = expr.children[0].GetChild(0);
        var assExpr = assignmentExpression switch
        {
            CParser.AssignmentExpressionContext ase => ParseAssignmentExpression(ase),
            _ => throw new ArgumentOutOfRangeException()
        };
        return new ExpressionStatement(assExpr);
    }

    private static Expression ParseAssignmentExpression(CParser.AssignmentExpressionContext ase)
    {
        var leftValue = ParseLValue(ase.GetChild(0));
        var expression = ParseExpression(ase.GetChild(2));

        return new AssignmentExpression(leftValue, expression);
    }

    private static LeftValue ParseLValue(IParseTree last)
    {
        return new LeftValue(last.GetText());
    }

    private static Expression ParseExpression(IParseTree expression)
    {
        var finalExpr = expression.Descendants<IParseTree>().Last();

        return finalExpr switch
        {
            TerminalNodeImpl terminalNodeImpl => int.TryParse(terminalNodeImpl.GetText(), out var n) ? new ConstantExpression(n) : new IdentifierExpression(terminalNodeImpl.GetText()),
            CParser.PrimaryExpressionContext primaryExpressionContext => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(expression), expression, null)
        };
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