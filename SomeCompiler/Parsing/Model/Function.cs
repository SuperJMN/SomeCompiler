namespace SomeCompiler.Parsing.Model;

public record Function(ReturnType ReturnType, string Identifier, ArgumentList ArgumentList, CompoundStatement CompoundStatement)
{
    public override string ToString()
    {
        return $"{ReturnType} {Identifier}({ArgumentList}) {CompoundStatement}";
    }
}