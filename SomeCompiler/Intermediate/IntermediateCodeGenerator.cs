using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using SomeCompiler.Compilation;
using SomeCompiler.Compilation.Model;

namespace SomeCompiler.Intermediate;

public class IntermediateCodeGenerator
{
    public Result<IntermediateCodeProgram, List<Error>> Generate(CompiledProgram result)
    {
        IEnumerable<IntermediateCode> instructions = new []
        {
            Call("Main"),
            Halt(),
            Label("Main"),
            Return(),
        };
        
        return Result.Success<IntermediateCodeProgram, List<Error>>(new IntermediateCodeProgram(instructions));
    }

    private IntermediateCode Return()
    {
        return new ReturnCode();
    }

    private IntermediateCode Label(string name)
    {
        return new LabelCode(name);
    }

    private IntermediateCode Halt()
    {
        return new HaltCode();
    }

    private IntermediateCode Call(string label)
    {
        return new CallCode(label);
    }
}

internal class CallCode : IntermediateCode
{
    public string LabelName { get; }

    public CallCode(string labelName)
    {
        LabelName = labelName;
    }

    public override string ToString() => $"Call {LabelName}";
}

internal class HaltCode : IntermediateCode
{
    public override string ToString() => "Halt";
}

internal class LabelCode : IntermediateCode
{
    public string Name { get; }

    public LabelCode(string name)
    {
        Name = name;
    }

    public override string ToString() => $"Label {Name}";
}

internal class ReturnCode : IntermediateCode
{
    public override string ToString() => "Return";
}

public class IntermediateCodeProgram : List<IntermediateCode>
{
    public IntermediateCodeProgram(IEnumerable<IntermediateCode> instructions) : base(instructions)
    {
    }

    public override string ToString() => this.Join(";");
}

public abstract class IntermediateCode
{
}