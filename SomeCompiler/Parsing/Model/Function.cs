namespace SomeCompiler.Parsing.Model;

public class Function
{
    public Function(ReturnType returnType, string identifier, ArgumentList argumentList, Block block)
    {
        ReturnType = returnType;
        Identifier = identifier;
        ArgumentList = argumentList;
        Block = block;
    }

    public ReturnType ReturnType { get; }
    public string Identifier { get; }
    public ArgumentList ArgumentList { get; }
    public Block Block { get; }

    public override string ToString() => $"{ReturnType} {Identifier}({ArgumentList}) {Block}";
}