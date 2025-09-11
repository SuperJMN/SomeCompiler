using CSharpFunctionalExtensions;
using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Generation.Intermediate.Model.Transforms;

using ModelCode = SomeCompiler.Generation.Intermediate.Model.Codes.Code;
using Ref = CodeGeneration.Model.Classes.Reference;

public class DeadCodeEliminationVisitor : IIntermediateTransform
{
    public Result<IntermediateCodeProgram> Run(IntermediateCodeProgram input)
    {
        var live = new HashSet<Ref>();
        var output = new List<ModelCode>();

        foreach (var code in input.AsEnumerable().Reverse())
        {
            var (uses, def, sideEffects) = Analyze(code);

            // Liveness before this instruction
            bool needed = sideEffects || (def != null && live.Contains(def)) || uses.Any(u => live.Contains(u));
            if (needed || def == null)
            {
                // Update liveness: live = (live - def) U uses
                if (def != null) live.Remove(def);
                foreach (var u in uses) live.Add(u);
                output.Add(code);
            }
            else
            {
                // Drop dead definition (pure and not live)
                if (def != null)
                {
                    // Still need to update liveness with uses
                    foreach (var u in uses) live.Add(u);
                }
            }
        }

        output.Reverse();
        return Result.Success(new IntermediateCodeProgram(output));
    }

    private static (IEnumerable<Ref> uses, Ref? def, bool sideEffects) Analyze(ModelCode code)
    {
        IEnumerable<Ref> uses = code switch
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

        Ref? def = code switch
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

        bool sideEffects = code is Call
                         || code is Return
                         || code is EmptyReturn
                         || code is Halt
                         || code is Jump
                         || code is BranchIfZero
                         || code is BranchIfNotZero
                         || code is Label
                         || code is LocalLabel
                         || code is LoadHLImm
                         || code is LoadHLRef
                         || code is Param
                         || code is ParamConst
                         || code is CleanArgs;

        // Pure defs: arithmetic ops and assignments
        return (uses, def, sideEffects);
    }
}
