using System.Text.RegularExpressions;
using EasyParse.Native;
using EasyParse.Native.Annotations;
using SomeCompiler.Parsing.Model;

namespace SomeCompiler.Parsing;

public class SomeGrammar : NativeGrammar
{
    protected override IEnumerable<Regex> IgnorePatterns =>
        new[] { new Regex(@"\s+") };

    public ArgumentType ArgumentType([L("int")] string keyword) => new ArgumentType(keyword);
    
    public ReturnType ReturnType([L("void")] string keyword) => new ReturnType(keyword);

    public int Unit([R("number", @"\d+")] string value) => int.Parse(value);

    public string Identifier([R("identifier", @"\w+")] string value) => value;

    public int Unit([L("-")] string minus, int unit) => -unit;

    public int Unit([L("(")] string open, int additive, [L(")")] string close) => additive;

    public int Multiplicative(int unit) => unit;

    public int Multiplicative(int multiplicative, [L("*", "/")] string op, int unit) =>
        op == "*" ? multiplicative * unit : multiplicative / unit;

    public int Additive(int multiplicative) => multiplicative;

    public int Additive(int additive, [L("+", "-")] string op, int multiplicative) =>
        op == "+" ? additive + multiplicative : additive - multiplicative;

    public LeftValue LeftValue(string identifier) => new(identifier);

    public Expression Expression(int additive) => new(additive);

    public Statement Statement([L("return")] string keyword, Expression expression,
        [L(";")] string semicolon) => new ReturnStatement(expression);

    public Statement Statement(LeftValue leftValue, [L("=")] string equals, Expression expression,
        [L(";")] string semicolon) =>
        new AssignmentStatement(leftValue, expression);

    public Statement Statement(ArgumentType argumentType, string identifier, [L(";")] string semicolon) =>
        new DeclarationStatement(argumentType, identifier);

    public Statements Statements(Statement statement) => new(new[] { statement });

    public Statements Statements(Statements statements, Statement statement) =>
        new(new[] { statement }.Concat(statements));


    public Block Block([L("{")] string open, Statements statements, [L("}")] string close) => new(statements);
    public Block Block([L("{")] string open, [L("}")] string close) => new(new Statements());

    public Functions Functions(Function function, Functions functions) => new(new[] { function }.Concat(functions));
    public Functions Functions(Function function) => new(new[] { function });

    public Function Function(ReturnType returnType, string identifier, [L("(")] string open, [L(")")] string close, Block block) =>
        new(returnType, identifier, new ArgumentList(), block);

    public Function Function(ReturnType returnType, string identifier, [L("(")] string open, ArgumentList argumentList, [L(")")] string close,
        Block block) => new(returnType, identifier, argumentList, block);

    public Argument Argument(ArgumentType argumentType, string identifier) => new(argumentType, identifier);
    public ArgumentList ArgumentList(Argument argument) => new(new[] { argument });

    public ArgumentList ArgumentList(Argument argument, [L(",")] string comma, ArgumentList argumentList) =>
        new(new[] { argument }.Concat(argumentList));

    [Start]
    public Program Program(Functions functions) => new(functions);
}