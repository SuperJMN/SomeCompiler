using CSharpFunctionalExtensions;
using RetroSharp.Generation.Intermediate.Model.Codes;

namespace RetroSharp.Generation.Intermediate.Model.Transforms;

using ModelCode = RetroSharp.Generation.Intermediate.Model.Codes.Code;
using Ref = CodeGeneration.Model.Classes.Reference;

public class BranchFoldingTransform : IIntermediateTransform
{
    private readonly Dictionary<Ref, int> consts = new();

    public Result<IntermediateCodeProgram> Run(IntermediateCodeProgram input)
    {
        consts.Clear();
        var output = new List<ModelCode>();

        foreach (var code in input)
        {
            switch (code)
            {
                case AssignConstant ac:
                    consts[ac.Target] = ac.Source;
                    output.Add(ac);
                    break;
                case Assign a:
                    if (consts.TryGetValue(a.Source, out var v))
                        consts[a.Target] = v;
                    else
                        consts.Remove(a.Target);
                    output.Add(a);
                    break;
                case Add or Subtract or Multiply or Divide or And or Or or AssignFromReturn:
                    // Any def kills constant knowledge of its target
                    KillDef(code);
                    output.Add(code);
                    break;
                case Call:
                    consts.Clear();
                    output.Add(code);
                    break;
                case BranchIfZero brz:
                    if (consts.TryGetValue(brz.Condition, out var val))
                    {
                        if (val == 0)
                        {
                            // Always jump
                            output.Add(new Jump(brz.Label));
                        }
                        else
                        {
                            // Never jump: remove
                        }
                    }
                    else
                    {
                        output.Add(code);
                    }
                    break;
                case BranchIfNotZero brnz:
                    if (consts.TryGetValue(brnz.Condition, out var val2))
                    {
                        if (val2 != 0)
                        {
                            output.Add(new Jump(brnz.Label));
                        }
                        else
                        {
                            // Never jump: remove
                        }
                    }
                    else
                    {
                        output.Add(code);
                    }
                    break;
                default:
                    output.Add(code);
                    break;
            }
        }

        return Result.Success(new IntermediateCodeProgram(output));
    }

    private void KillDef(ModelCode code)
    {
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
        if (def != null)
            consts.Remove(def);
    }
}
