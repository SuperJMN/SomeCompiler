using CSharpFunctionalExtensions.ValueTasks;
using SomeCompiler.Binding;

namespace SomeCompiler.Binding2;

public abstract class Node
{
    public Node(Maybe<Node> parent)
    {
        //this.Scope = new Scope(parent);
    }

    public Scope Scope { get; set; }

    public void Declare(string name, SymbolType type)
    {
        //Scope.TryDeclare()
        //    .TapError(Error);
    }
    
    private void Error(string error)
    {
        Errors.Add(error);
    }

    public IEnumerable<(string, SymbolType)> Symbols => Scope.Symbols;
    //public Result<SymbolType> Get(string name) => Scope.Get(name);

    public List<string> Errors { get; } = [];
}

//public class Program : Node
//{
    
//}

public abstract class SymbolType
{
    public abstract string Name { get; }
}
public class IntType : SymbolType
{
    public static readonly IntType Instance = new IntType();
    private IntType() { }
    public override string Name => "int";
}


public class Scope : IDisposable
{
    private readonly Dictionary<string, SymbolType> localSymbols = new();

    public Scope(Maybe<Scope> parent)
    {
        Parent = parent;
    }

    public IEnumerable<(string, SymbolType)> Symbols => localSymbols.Select(pair => (pair.Key, pair.Value));

    public Maybe<SymbolType> Get(string name)
    {
        return localSymbols.TryFind(name).Or(Parent.Bind(p => p.Get(name)));
    }

    public Maybe<Scope> Parent { get; }

    public Result TryDeclare(string name, SymbolType symbol)
    {
        return Get(name)
            .Match(type => Result.Failure($"{symbol}({type}) is already declared"), Result.Success)
            .Tap(() => localSymbols.Add(name, symbol));
    }

    public Scope Create()
    {
        return new Scope(this);
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}