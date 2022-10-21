using EasyParse.Fluent.Symbols;
using EasyParse.Fluent;
using SomeCompiler.Parsing.Model;

namespace SomeCompiler.Parsing;

public class SomeGrammar : FluentGrammar
{
    private NonTerminal Parameter => () => Rule()
        .Match("int", Symbols.Identifier).To<string, Argument>(b => new Argument(new ArgumentType("int"), b));

    private NonTerminal ParameterList => () => Rule()
        .Match(Parameter).To<Argument, ArgumentList>(a => new ArgumentList { a })
        .Match(ParameterList, ",", Parameter).To<ArgumentList, Argument, ArgumentList>((aa, a) => new ArgumentList(aa) { a });

    private NonTerminal Unit => () => Rule()
        .Match(Pattern.Int).To((int i) => new Unit(null, new ConstantExpression(i)))
        .Match("-", Unit).To((Unit u) => new Unit("-", u))
        .Match(Symbols.Identifier).To((string e) => new Unit(null, new IdentifierExpression(e)))
        .Match("(", Term, ")").To((Term term) => new Unit(null, term));

    private NonTerminal Factor => () => Rule()
        .Match(Unit).To((Unit u) => new Factor(null, u))
        .Match(Factor, "*", Unit).To((Factor f, Unit u) => new Factor("*", f, u))
        .Match(Factor, "/", Unit).To((Factor f, Unit u) => new Factor("/", f, u));

    private NonTerminal Term => () => Rule()
        .Match(Factor).To((Factor f) => new Term(null, f))
        .Match(Term, "+", Factor).To((Term t, Factor f) => new Term("+", t, f))
        .Match(Term, "-", Factor).To((Term t, Factor f) => new Term("-", t, f));

    private NonTerminal LeftValue => () => Rule()
        .Match(Symbols.Identifier).To((string x) => new LeftValue(x));
    
    private NonTerminal Expression => () => Rule()
        .Match<Expression>(Term)
        .Match(LeftValue, "=", Expression).To<LeftValue, Expression, Expression>((a, b) => new AssignmentExpression(a, b));

    private NonTerminal CompoundStatement => () => Rule()
        .Match(Symbols.EmptyBlock).To((string _) => new Block())
        .Match("{", Statements, "}").To((Statements ss) => new Block(ss));

    private NonTerminal Statements => () => Rule()
        .Match(Statement).To((Statement s) => new Statements { s })
        .Match(Statements, Statement).To((Statements ss, Statement s) => new Statements(ss) { s });
        

    private NonTerminal Functions => () => Rule()
        .Match(Function).To((Function s) => new Functions() { s })
        .Match(Functions, Function).To((Functions ff, Function f) => new Functions(ff) { f });

    private NonTerminal Statement => () => Rule()
        .Match(Expression, ";").To((Expression e) => new ExpressionStatement(e))
        .Match("return", Expression, ";").To((Expression e) => (Statement)new ReturnStatement(e));

    private NonTerminal Function => () => Rule()
        .Match("void", Symbols.Identifier, "(", ParameterList, ")", CompoundStatement).To((string i, ArgumentList args, Block c) => new Function(i, args, c))
        .Match("void", Symbols.Identifier, "(", ")", CompoundStatement).To((string i, Block c) => new Function(i, new ArgumentList(), c))
    ;

    private NonTerminal Program => () => Rule()
        .Match(Functions).To((Functions s) => new Program(s));

    protected override IEnumerable<RegexSymbol> Ignore => new List<RegexSymbol> { Pattern.WhiteSpace };

    protected override IRule Start => Program();
}