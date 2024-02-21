using Antlr4.Runtime;
using CSharpFunctionalExtensions;
using System.Xml.Linq;
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
        return new FunctionSyntax(functionContext.type().GetText(), functionContext.IDENTIFIER().ToString()!, ParseParameters(functionContext.parameters()).ToList(), block);
    }

    private IEnumerable<ParameterSyntax> ParseParameters(ParametersContext parameters)
    {
        if (parameters is { } parameterContexts)
        {
            return parameterContexts.parameter().Select(ParseParameter);
        }

        return Enumerable.Empty<ParameterSyntax>();
    }

    private ParameterSyntax ParseParameter(ParameterContext parameterContext) => new(parameterContext.type().GetText(), parameterContext.IDENTIFIER().GetText());

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

        if (statementContext.variableDeclaration() is { } declaration)
        {
            return ParseDeclaration(declaration);
        }

        if (statementContext.returnStatement() is { } returnStatement)
        {
            return ParseReturn(returnStatement);
        }

        throw new InvalidOperationException();
    }

    private StatementSyntax ParseReturn(ReturnStatementContext returnStatement)
    {
        var maybeExpression = Maybe.From(returnStatement.expression() is { } expr ? ParseExpression(expr) : null);
        return new ReturnSyntax(maybeExpression);
    }

    private StatementSyntax ParseDeclaration(VariableDeclarationContext declaration)
    {
        var maybeInitialization = Maybe.From(declaration.expression() is { } expr ? ParseExpression(expr) : null);
        return new DeclarationSyntax(declaration.type().GetText(), declaration.IDENTIFIER().GetText(), maybeInitialization);
    }

    private StatementSyntax ParseFunctionCall(FunctionCallContext functionCall)
    {
        var name = functionCall.IDENTIFIER().ToString()!;
        var arguments = functionCall.arguments().expression().Select(ParseExpression);
        ;
        return new ExpressionStatementSyntax(new FunctionCall(name, arguments));
    }

    private StatementSyntax ParseWhileLoop(WhileLoopContext whileLoop) => throw new NotImplementedException();

    private StatementSyntax ParseConditional(ConditionalContext conditional) => throw new NotImplementedException();

    private StatementSyntax ParseAssignment(AssignmentContext assignment)
    {
        var lvalue = ParseLValue(assignment.IDENTIFIER().GetText());
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

    private LValue ParseLValue(string identifier) => new IdentifierLValue(identifier);

    private ExpressionSyntax ParseAddExpression(AddExpressionContext addExpression)
    {
        if (addExpression.addExpression() is { } addExpr)
        {
            var left = ParseAddExpression(addExpr);
            var right = ParseMultExpression(addExpression.mulExpression());
            return new AddExpression(left, right);
        }

        return ParseMultExpression(addExpression.mulExpression());
    }

    private ExpressionSyntax ParseMultExpression(MulExpressionContext mulExpression)
    {
        if (mulExpression.mulExpression() is {} multExpr)
        {
            var left = ParseMultExpression(multExpr);
            var right = ParseAtom(mulExpression.atom());
            return new MultExpression(left, right);
        }
        
        if (mulExpression.atom() is { } atom)
        {
            return ParseAtom(atom);
        }

        return new MultExpression(ParseExpression((ExpressionContext)mulExpression.children[0]), ParseExpression((ExpressionContext)mulExpression.children[1]));
    }

    private ExpressionSyntax ParseAtom(AtomContext atom)
    {
        if (atom.LITERAL() is { } node)
        {
            return new ConstantSyntax(node.GetText());
        }

        if (atom.IDENTIFIER() is { } identifier)
        {
            return new IdentifierSyntax(identifier.GetText());
        }

        throw new NotImplementedException();
    }
}

public class IdentifierSyntax : ExpressionSyntax
{
    public string Identifier { get; }

    public IdentifierSyntax(string identifier)
    {
        Identifier = identifier;
    }

    public override void Accept(ISyntaxVisitor visitor)
    {
        visitor.VisitIdentifier(this);
    }
}

public class ReturnSyntax : StatementSyntax
{
    public Maybe<ExpressionSyntax> Expression { get; }

    public ReturnSyntax(Maybe<ExpressionSyntax> expression)
    {
        Expression = expression;
    }

    public override void Accept(ISyntaxVisitor visitor)
    {
        visitor.VisitReturn(this);
    }
}

public class DeclarationSyntax : StatementSyntax
{
    public DeclarationSyntax(string type, string name, Maybe<ExpressionSyntax> initialization)
    {
        Type = type;
        Name = name;
        Initialization = initialization;
    }

    public string Type { get; }
    public string Name { get; }
    public Maybe<ExpressionSyntax> Initialization { get; }

    public override void Accept(ISyntaxVisitor visitor)
    {
        visitor.VisitDeclaration(this);
    }
}