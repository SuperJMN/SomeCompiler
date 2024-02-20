using System.Collections;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CSharpFunctionalExtensions;
using Zafiro.Core.Mixins;
using static SomeCompiler.Parser.SomeLanguageParser;

namespace SomeCompiler.Parser;

public class SomeParser
{
    public Result<ProgramSyntax> Parse(string input) => Tokenize(input)
        .Bind(Parse)
        .Map(ParseProgram);

    private static Result<ProgramContext> Parse(CommonTokenStream tokenStream)
    {
        var parser = new SomeLanguageParser(tokenStream);
        var listenerLexer = new ErrorListener<IToken>();
        parser.AddErrorListener(listenerLexer);
        var result = parser.program();

        if (listenerLexer.Errors.Any())
        {
            return Result.Failure<ProgramContext>(listenerLexer.Errors.Select(x => x.ToString()).JoinWithLines());
        }

        return result;
    }

    private static Result<CommonTokenStream> Tokenize(string input)
    {
        var lexer = new SomeLanguageLexer(CharStreams.fromString(input));
        var listenerLexer = new ErrorListener<int>();
        lexer.AddErrorListener(listenerLexer);

        var tokenStream = new CommonTokenStream(lexer);

        if (listenerLexer.Errors.Any())
        {
            return Result.Failure<CommonTokenStream>(listenerLexer.Errors.Select(x => x.ToString()).JoinWithLines());
        }

        return tokenStream;
    }

    private ProgramSyntax ParseProgram(ProgramContext program)
    {
        var funcs = program.function().Select(f => ParseFunction(f));
        return new ProgramSyntax(funcs.ToList());
    }

    private FunctionSyntax ParseFunction(FunctionContext functionContext)
    {
        var block = ParseBlock(functionContext.block());
        return new FunctionSyntax(functionContext.type().GetText(), functionContext.IDENTIFIER().ToString(), block);
    }

    private BlockSyntax ParseBlock(BlockContext block)
    {
        var statements = block.statement().Select(ParseStatement);
        return new BlockSyntax(statements.ToList());
    }

    private StatementSyntax ParseStatement(StatementContext statementContext)
    {
        if (statementContext.assignment() is { } assignment)
        {
            return ParseAssignment(assignment);
        }

        if (statementContext.conditional() is { } conditional)
        {
            return ParseConditional(conditional);
        }

        if (statementContext.whileLoop() is { } whileLoop)
        {
            return ParseWhileLoop(whileLoop);
        }

        if (statementContext.functionCall() is { } functionCall)
        {
            return ParseFunctionCall(functionCall);
        }

        throw new InvalidOperationException();
    }

    private StatementSyntax ParseFunctionCall(FunctionCallContext functionCall)
    {
        var name = functionCall.IDENTIFIER().ToString()!;
        var arguments = functionCall.arguments().expression().Select(ParseExpression);;
        return new ExpressionStatementSyntax(new FunctionCall(name, arguments));
    }

    private StatementSyntax ParseWhileLoop(WhileLoopContext whileLoop) => throw new NotImplementedException();

    private StatementSyntax ParseConditional(ConditionalContext conditional) => throw new NotImplementedException();

    private StatementSyntax ParseAssignment(AssignmentContext assignment)
    {
        LValue lvalue = new IdentifierLValue(assignment.IDENTIFIER().ToString());
        return new AssignmentSyntax(lvalue, ParseExpression(assignment.expression()));
    }

    private ExpressionSyntax ParseExpression(ExpressionContext expression)
    {
        if (expression.addExpression() is { } addExpression)
        {
            return ParseAddExpression(addExpression);
        }

        throw new NotImplementedException(expression.ToString());
    }

    private LValue ParseLValue(IParseTree addExpressionChild)
    {
        return new IdentifierLValue(addExpressionChild.GetText());
    }

    private ExpressionSyntax ParseAddExpression(AddExpressionContext addExpression)
    {
        if (addExpression.mulExpression() is {} multExpression)
        {
            return ParseMultExpression(multExpression);
        }

        var left = ParseExpression((ExpressionContext) addExpression.children[0]);
        var right = ParseExpression((ExpressionContext) addExpression.children[1]);
        return new AddExpression(left, right);
    }

    private ExpressionSyntax ParseMultExpression(MulExpressionContext mulExpression)
    {
        if (mulExpression.atom() is { } atom)
        {
            return ParseAtom(mulExpression.atom());
        }
        return new MultExpression(ParseExpression((ExpressionContext) mulExpression.children[0]), ParseExpression((ExpressionContext) mulExpression.children[1]));
    }

    private ExpressionSyntax ParseAtom(AtomContext atom)
    {
        if (atom.LITERAL() is { } node)
        {
            return new ConstantSyntax(node.GetText());
        }

        throw new NotImplementedException();
    }
}