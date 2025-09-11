using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Generation.Intermediate.Model.Visitors;

public class PrettyPrinterVisitor : IModelCodeVisitor<string>
{
    private readonly Dictionary<CodeGeneration.Model.Classes.Reference, string> map;

    public PrettyPrinterVisitor(Dictionary<CodeGeneration.Model.Classes.Reference, string> map)
    {
        this.map = map;
    }

    public string VisitAdd(Add code) => code.ToString(map);
    public string VisitSubtract(Subtract code) => code.ToString(map);
    public string VisitMultiply(Multiply code) => code.ToString(map);
    public string VisitDivide(Divide code) => code.ToString(map);
    public string VisitAnd(And code) => code.ToString(map);
    public string VisitOr(Or code) => code.ToString(map);

    public string VisitAssign(Assign code) => code.ToString(map);
    public string VisitAssignConstant(AssignConstant code) => code.ToString(map);
    public string VisitAssignFromReturn(AssignFromReturn code) => code.ToString(map);

    public string VisitLabel(Label code) => code.ToString(map);
    public string VisitLocalLabel(LocalLabel code) => code.ToString(map);
    public string VisitBranchIfZero(BranchIfZero code) => code.ToString(map);
    public string VisitBranchIfNotZero(BranchIfNotZero code) => code.ToString(map);
    public string VisitJump(Jump code) => code.ToString(map);

    public string VisitCall(Call code) => code.ToString(map);
    public string VisitReturn(Return code) => code.ToString(map);
    public string VisitEmptyReturn(EmptyReturn code) => code.ToString(map);

    public string VisitHalt(Halt code) => code.ToString(map);
    public string VisitLoadHLImm(LoadHLImm code) => code.ToString(map);
    public string VisitLoadHLRef(LoadHLRef code) => code.ToString(map);
    public string VisitParam(Param code) => code.ToString(map);
    public string VisitParamConst(ParamConst code) => code.ToString(map);
    public string VisitCleanArgs(CleanArgs code) => code.ToString(map);

    public string VisitDefault(Code code) => code.ToString(map);

    public static string Print(IntermediateCodeProgram program)
    {
        // Build stable naming map (Named keep name; Placeholder -> T1, T2, ...)
        var named = program.NamedReferences().Select(x => ((CodeGeneration.Model.Classes.Reference)x, x.Value));
        var unnamed = program.UnnamedReferences().Select((x, i) => (x, $"T{i + 1}"));
        var map = named.Concat(unnamed).ToDictionary(x => x.Item1, tuple => tuple.Item2);

        var visitor = new PrettyPrinterVisitor(map);
        var lines = program.Select(code => code.Accept(visitor));
        return string.Join("\n", lines);
    }
}
