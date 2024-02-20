namespace SomeCompiler.Parser;

public class ProgramSyntax
{
    public IList<FunctionSyntax> Functions { get; }

    public ProgramSyntax(IList<FunctionSyntax> functions)
    {
        Functions = functions;
    }
}

public class FunctionSyntax
{
    public BlockSyntax Block { get; }

    public FunctionSyntax(BlockSyntax block)
    {
        Block = block;
    }
}