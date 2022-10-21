using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using EasyParse.Native;
using EasyParse.Native.Annotations;
using SomeCompiler.Parsing.Model;

namespace SomeCompiler.Parsing;

//public class SomeGrammarBackup : NativeGrammar
//{
//    protected override IEnumerable<Regex> IgnorePatterns => new[] { new Regex(@"\s+") };

//    public int Number([R("number", @"\d+")] string value) => int.Parse(value);
//    public string Identifier([R("identifier", @"\w+")] string value) => value;
//    public ReturnType ReturnType([L("void")] string keyword) => new(keyword);
//    public ArgumentType ArgumentType([L("int")] string keyword) => new(keyword);
//    public LeftValue LeftValue(string identifier) => new(identifier);
//    public ReturnKeyword ReturnKeyword([L("return")] string keyword) => new ReturnKeyword();

//    public AddOperator AddOperator([L("+")] string plus) => new AddOperator();
//    public SubtractOperator SubtractOperator([L("-")] string plus) => new SubtractOperator();

//    public AddendOperator AddendOperator(AddOperator addOperator) => addOperator;
//    public AddendOperator AddendOperator(SubtractOperator subtractOperator) => subtractOperator;

//    public Statements Statements(Statement statement) => new(statement);
//    public Statements Statements(Statement statement, Statements statements) => new(new[]{ statement }.Concat(statements));
    
//    public Addend Addend(Expression expression1, AddendOperator addendOperator, Expression expression2) => new Addend(expression1, addendOperator, expression2);

//    public Expression Expression(int number) => new ConstantExpression(number);
//    public Expression Expression(string identifier) => new IdentifierExpression(identifier);
//    public Expression Expression(Addend addend) => addend;
    
//    public Expression Expression(LeftValue leftValue, [L("=")] string equals, Expression expression) => new AssignmentExpression(leftValue, expression);
//    public Expression Expression([L("-")] string minus, Expression expression) => new NegateExpression(expression);

//    public CompoundStatement CompoundStatement([L("{")] string openBrace, Statements statements, [L("}")] string closeBrace) => new CompoundStatement(statements);
//    public CompoundStatement CompoundStatement([L("{")] string openBrace, [L("}")] string closeBrace) => new CompoundStatement(new Statements());

//    public Statement Statement(ArgumentType argumentType, string identifier, [L(";")] string semicolon) => new DeclarationStatement(argumentType, identifier);
//    public Statement Statement(Expression expression, [L(";")] string semicolon) => new ExpressionStatement(expression);
//    public Statement Statement(ReturnKeyword returnKeyword, [L(";")] string semicolon) => new ReturnStatement(Maybe<Expression>.None);
//    public Statement Statement(ReturnKeyword returnKeyword, Expression expression, [L(";")] string semicolon) => new ReturnStatement(expression);

//    public Function Function(ReturnType returnType, string identifier, [L("(")] string lparen, [L(")")] string rparen, CompoundStatement compoundStatement) => new Function(returnType, identifier, new ArgumentList(), compoundStatement);
//    public Function Function(ReturnType returnType, string identifier, [L("(")] string lparen, ArgumentList argumentList, [L(")")] string rparen, CompoundStatement compoundStatement) => new Function(returnType, identifier, argumentList, compoundStatement);

//    public Functions Functions(Function function) => new(function);
//    public Functions Functions(Function function, Functions functions) => new(new[]{ function }.Concat(functions));

//    public Argument Argument(ArgumentType argumentType, string identifier) => new(argumentType, identifier);

//    public ArgumentList ArgumentList(Argument argument) => new(new[] { argument });
//    public ArgumentList ArgumentList(Argument argument, [L(",")] string comma, ArgumentList argumentList) => new(new[] { argument }.Concat(argumentList));

//    [Start]
//    public Program Program(Functions functions) => new Program(functions);
//}