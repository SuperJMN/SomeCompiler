using CSharpFunctionalExtensions;

namespace SomeCompiler.SemanticAnalysis;

public class Scope
{
    public Maybe<Scope> Parent { get; }
    private readonly Dictionary<string, Symbol> variables;

    public Scope(Maybe<Scope> parent, Maybe<Dictionary<string, Symbol>> variables)
    {
        Parent = parent;
        this.variables = variables.GetValueOrDefault(new Dictionary<string, Symbol>());
    }

    public Scope(Scope parent) : this(parent, new Dictionary<string, Symbol>())
    {
    }

    public Result<Scope> TryDeclare(Symbol symbol)
    {
        if (variables.ContainsKey(symbol.Name))
        {
            // La variable ya existe en este alcance, devolver el mismo alcance
            return Result.Failure<Scope>($"'{symbol}' is already declared");
        }
        else
        {
            // Crear un nuevo diccionario con la nueva variable
            var newVariables = new Dictionary<string, Symbol>(variables)
            {
                [symbol.Name] = symbol
            };
            // Devolver un nuevo alcance con las nuevas variables
            return new Scope(Parent, newVariables);
        }
    }

    public Maybe<Symbol> Get(string name)
    {
        return variables
            .TryFind(name)
            .Or(() => Parent.Bind(scope => scope.Get(name)));
    }

    public static Scope Empty = new Scope(Maybe<Scope>.None, Maybe<Dictionary<string, Symbol>>.None);
}