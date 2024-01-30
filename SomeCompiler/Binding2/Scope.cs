namespace SomeCompiler.Binding2;

public class Scope
{
    public Maybe<Scope> Parent { get; }
    private readonly Dictionary<string, SymbolType> variables;

    public Scope(Maybe<Scope> parent, Maybe<Dictionary<string, SymbolType>> variables)
    {
        Parent = parent;
        this.variables = variables.GetValueOrDefault(new Dictionary<string, SymbolType>());
    }

    public Scope(Scope parent) : this(parent, new Dictionary<string, SymbolType>())
    {
    }

    public Result<Scope> TryDeclare(string name, SymbolType type)
    {
        if (variables.ContainsKey(name))
        {
            // La variable ya existe en este alcance, devolver el mismo alcance
            return Result.Failure<Scope>("Symbol is declared");
        }
        else
        {
            // Crear un nuevo diccionario con la nueva variable
            var newVariables = new Dictionary<string, SymbolType>(variables)
            {
                [name] = type
            };
            // Devolver un nuevo alcance con las nuevas variables
            return new Scope(Parent, newVariables);
        }
    }

    public Maybe<SymbolType> Get(string name)
    {
        return variables
            .TryFind(name)
            .Or(() => Parent.Bind(scope => scope.Get(name)));
    }

    public static Scope Empty = new Scope(Maybe<Scope>.None, Maybe<Dictionary<string, SymbolType>>.None);
}
