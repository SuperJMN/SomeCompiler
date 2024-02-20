using Antlr4.Runtime;
using CSharpFunctionalExtensions;
using Zafiro.Core.Mixins;
using static SomeCompiler.Parser.SomeLanguageParser;

namespace SomeCompiler.Parser;

public class SomeParser
{
    public Result<ProgramSyntax> Parse(string input)
    {
        return Tokenize(input)
            .Bind(Parse)
            .Map(ParseProgram);
    }

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
        return new FunctionSyntax(block);
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
        return new ExpressionStatementSyntax(new FunctionCall());
    }

    private StatementSyntax ParseWhileLoop(WhileLoopContext whileLoop)
    {
        throw new NotImplementedException();
    }

    private StatementSyntax ParseConditional(ConditionalContext conditional)
    {
        throw new NotImplementedException();
    }

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

    private ExpressionSyntax ParseAddExpression(AddExpressionContext addExpression)
    {
        return ParseMultExpression(addExpression.mulExpression());
    }

    private ExpressionSyntax ParseMultExpression(MulExpressionContext mulExpression)
    {
        return new ExpressionSyntax();
    }
}

internal class ExpressionStatementSyntax : StatementSyntax
{
    public ExpressionSyntax Expression { get; }

    public ExpressionStatementSyntax(ExpressionSyntax expression)
    {
        Expression = expression;
    }
}

internal class FunctionCall : ExpressionSyntax
{
}

internal class AssignmentSyntax : StatementSyntax
{
    public LValue Lvalue { get; }
    public ExpressionSyntax Expression { get; }

    public AssignmentSyntax(LValue lvalue, ExpressionSyntax expression)
    {
        Lvalue = lvalue;
        Expression = expression;
    }
}

internal class ExpressionSyntax
{
}

public abstract class LValue
{
    
}

internal class IdentifierLValue : LValue
{
    public string Identifier { get; }

    public IdentifierLValue(string identifier)
    {
        Identifier = identifier;
    }
}

public class StatementSyntax
{
}

public class BlockSyntax
{
    public List<StatementSyntax> Statements { get; }

    public BlockSyntax(List<StatementSyntax> statements)
    {
        Statements = statements;
    }
}