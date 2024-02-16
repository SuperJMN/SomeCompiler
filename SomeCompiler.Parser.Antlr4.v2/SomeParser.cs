using Antlr4.Runtime;

namespace SomeCompiler.Parser;

public class SomeParser
{
    public ProgramSyntax Parse(string input)
    {
        var lexer = new SomeLanguageLexer(CharStreams.fromString(input));
        var parser = new SomeLanguageParser(new CommonTokenStream(lexer));
        return ParseProgram(parser.program());
    }

    private ProgramSyntax ParseProgram(SomeLanguageParser.ProgramContext program)
    {
        var funcs = program.function().Select(f => ParseFunction(f));
        return new ProgramSyntax(funcs.ToList());
    }

    private FunctionSyntax ParseFunction(SomeLanguageParser.FunctionContext functionContext)
    {
        var block = ParseBlock(functionContext.block());
        return new FunctionSyntax(block);
    }

    private BlockSyntax ParseBlock(SomeLanguageParser.BlockContext block)
    {
        var statements = block.statement().Select(ParseStatement);
        return new BlockSyntax(statements.ToList());
    }

    private StatementSyntax ParseStatement(SomeLanguageParser.StatementContext statementContext)
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
            return ParseFunctionaCall(functionCall);
        }

        throw new InvalidOperationException();
    }

    private StatementSyntax ParseFunctionaCall(SomeLanguageParser.FunctionCallContext functionCall)
    {
        throw new NotImplementedException();
    }

    private StatementSyntax ParseWhileLoop(SomeLanguageParser.WhileLoopContext whileLoop)
    {
        throw new NotImplementedException();
    }

    private StatementSyntax ParseConditional(SomeLanguageParser.ConditionalContext conditional)
    {
        throw new NotImplementedException();
    }

    private StatementSyntax ParseAssignment(SomeLanguageParser.AssignmentContext assignment)
    {
        throw new NotImplementedException();
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