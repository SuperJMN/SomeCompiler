using CSharpFunctionalExtensions;
using SomeCompiler.Generation.Intermediate.Model.Codes;
using SomeCompiler.Generation.Intermediate.Model.Visitors;

namespace SomeCompiler.Generation.Intermediate.Model.Transforms;

using ModelCode = SomeCompiler.Generation.Intermediate.Model.Codes.Code;
using Ref = CodeGeneration.Model.Classes.Reference;

public class CopyPropagationVisitor : IIntermediateTransform, IModelCodeVisitor<ModelCode>
{
    private readonly Dictionary<Ref, Ref> subs = new();

    public Result<IntermediateCodeProgram> Run(IntermediateCodeProgram input)
    {
        subs.Clear();
        var output = new List<ModelCode>();
        foreach (var code in input)
        {
            var rewritten = code.Accept(this);
            output.Add(rewritten);
        }
        return Result.Success(new IntermediateCodeProgram(output));
    }

    private Ref Resolve(Ref r)
    {
        while (subs.TryGetValue(r, out var to))
        {
            if (ReferenceEquals(to, r)) break;
            r = to;
        }
        return r;
    }

    private static bool Defines(ModelCode code, out Ref? def)
    {
        def = code switch
        {
            Assign a => a.Target,
            AssignConstant ac => ac.Target,
            AssignFromReturn afr => afr.Target,
            Add a => a.Target,
            Subtract s => s.Target,
            Multiply m => m.Target,
            Divide d => d.Target,
            And a => a.Target,
            Or o => o.Target,
            _ => null
        };
        return def != null;
    }

    private static IEnumerable<Ref> Uses(ModelCode code)
        => code switch
        {
            Assign a => new[] { a.Source },
            Add a => new[] { a.Left, a.Right },
            Subtract s => new[] { s.Left, s.Right },
            Multiply m => new[] { m.Left, m.Right },
            Divide d => new[] { d.Left, d.Right },
            And a => new[] { a.Left, a.Right },
            Or o => new[] { o.Left, o.Right },
            Return r => new[] { r.Reference },
            BranchIfZero b => new[] { b.Condition },
            BranchIfNotZero b => new[] { b.Condition },
            LoadHLRef l => new[] { l.From },
            Param p => new[] { p.Argument },
            _ => Array.Empty<Ref>()
        };

    private ModelCode RewriteSources(ModelCode code)
    {
        return code switch
        {
            Assign a => new Assign(a.Target, Resolve(a.Source)),
            AssignConstant ac => ac,
            AssignFromReturn afr => afr,
            Add a => new Add(a.Target, Resolve(a.Left), Resolve(a.Right)),
            Subtract s => new Subtract(s.Target, Resolve(s.Left), Resolve(s.Right)),
            Multiply m => new Multiply(m.Target, Resolve(m.Left), Resolve(m.Right)),
            Divide d => new Divide(d.Target, Resolve(d.Left), Resolve(d.Right)),
            And a => new And(a.Target, Resolve(a.Left), Resolve(a.Right)),
            Or o => new Or(o.Target, Resolve(o.Left), Resolve(o.Right)),
            Return r => new Return(Resolve(r.Reference)),
            BranchIfZero b => new BranchIfZero(Resolve(b.Condition), b.Label),
            BranchIfNotZero b => new BranchIfNotZero(Resolve(b.Condition), b.Label),
            LoadHLRef l => new LoadHLRef(Resolve(l.From)),
            Param p => new Param(Resolve(p.Argument)),
            _ => code
        };
    }

    private void Invalidate(Ref r)
    {
        if (subs.ContainsKey(r)) subs.Remove(r);
    }

    public ModelCode VisitAssign(Assign code)
    {
        var rewritten = (Assign)RewriteSources(code);
        // Update substitution: target := source
        subs[rewritten.Target] = rewritten.Source;
        return rewritten;
    }

    public ModelCode VisitAssignConstant(AssignConstant code)
    {
        Invalidate(code.Target);
        return code;
    }

    public ModelCode VisitAssignFromReturn(AssignFromReturn code)
    {
        Invalidate(code.Target);
        return code;
    }

    public ModelCode VisitAdd(Add code)
    {
        if (Defines(code, out var def) && def != null) Invalidate(def);
        return RewriteSources(code);
    }

    public ModelCode VisitSubtract(Subtract code)
    {
        if (Defines(code, out var def) && def != null) Invalidate(def);
        return RewriteSources(code);
    }

    public ModelCode VisitMultiply(Multiply code)
    {
        if (Defines(code, out var def) && def != null) Invalidate(def);
        return RewriteSources(code);
    }

    public ModelCode VisitDivide(Divide code)
    {
        if (Defines(code, out var def) && def != null) Invalidate(def);
        return RewriteSources(code);
    }

    public ModelCode VisitAnd(And code)
    {
        if (Defines(code, out var def) && def != null) Invalidate(def);
        return RewriteSources(code);
    }

    public ModelCode VisitOr(Or code)
    {
        if (Defines(code, out var def) && def != null) Invalidate(def);
        return RewriteSources(code);
    }

    public ModelCode VisitLabel(Label code) => code;
    public ModelCode VisitLocalLabel(LocalLabel code) => code;
    public ModelCode VisitBranchIfZero(BranchIfZero code) => RewriteSources(code);
    public ModelCode VisitBranchIfNotZero(BranchIfNotZero code) => RewriteSources(code);
    public ModelCode VisitJump(Jump code) => code;

    public ModelCode VisitCall(Call code)
    {
        // Calls may clobber unknown refs; clear substitutions conservatively
        subs.Clear();
        return code;
    }

    public ModelCode VisitReturn(Return code) => RewriteSources(code);
    public ModelCode VisitEmptyReturn(EmptyReturn code) => code;
    public ModelCode VisitHalt(Halt code) => code;
    public ModelCode VisitLoadHLImm(LoadHLImm code) => code;
    public ModelCode VisitLoadHLRef(LoadHLRef code) => RewriteSources(code);
    public ModelCode VisitParam(Param code) => RewriteSources(code);
    public ModelCode VisitParamConst(ParamConst code) => code;
    public ModelCode VisitCleanArgs(CleanArgs code) => code;

    public ModelCode VisitDefault(ModelCode code) => code;
}
