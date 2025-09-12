using CSharpFunctionalExtensions;
using RetroSharp.Generation.Intermediate.Model.Codes;

namespace RetroSharp.Generation.Intermediate.Model.Transforms;

using ModelCode = RetroSharp.Generation.Intermediate.Model.Codes.Code;
using Ref = CodeGeneration.Model.Classes.Reference;

public class StrengthReductionTransform : IIntermediateTransform
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
                case Multiply m:
                {
                    // Try to reduce multiply by constant
                    if (TryGetConst(m.Left, out var lc) || TryGetConst(m.Right, out var rc))
                    {
                        bool leftConst = TryGetConst(m.Left, out lc);
                        bool rightConst = TryGetConst(m.Right, out rc);
                        if (leftConst && rightConst)
                        {
                            // Both consts -> should have been folded earlier, but handle anyway
                            output.Add(new AssignConstant(m.Target, lc * rc));
                            consts[m.Target] = lc * rc;
                            break;
                        }

                        int c = leftConst ? lc : rc;
                        var varRef = leftConst ? m.Right : m.Left;

                        if (c == 0)
                        {
                            output.Add(new AssignConstant(m.Target, 0));
                            consts[m.Target] = 0;
                            break;
                        }
                        if (c == 1)
                        {
                            output.Add(new Assign(m.Target, varRef));
                            consts.Remove(m.Target);
                            break;
                        }
                        if (IsPowerOfTwo(c))
                        {
                            // Build sequence of adds by doubling
                            var steps = BuildDoublingAdds(varRef, m.Target, c);
                            output.AddRange(steps);
                            consts.Remove(m.Target);
                            break;
                        }
                    }
                    // Fallback: keep as-is
                    output.Add(m);
                    consts.Remove(m.Target);
                    break;
                }
                default:
                {
                    // Kill def knowledge for any other def
                    Ref? def = code switch
                    {
                        Add a => a.Target,
                        Subtract s => s.Target,
                        Divide d => d.Target,
                        And a => a.Target,
                        Or o => o.Target,
                        AssignFromReturn afr => afr.Target,
                        _ => null
                    };
                    if (def != null) consts.Remove(def);

                    // Calls clobber
                    if (code is Call) consts.Clear();

                    output.Add(code);
                    break;
                }
            }
        }

        return Result.Success(new IntermediateCodeProgram(output));
    }

    private static bool IsPowerOfTwo(int x) => x > 0 && (x & (x - 1)) == 0;

    private bool TryGetConst(Ref r, out int v) => consts.TryGetValue(r, out v);

    private static IEnumerable<ModelCode> BuildDoublingAdds(Ref source, Ref target, int factor)
    {
        // factor is power of two: 2^k
        int k = (int)Math.Log2(factor);
        Ref accumTarget = target;
        if (k == 1)
        {
            // target = source + source
            yield return new Add(target, source, source);
            yield break;
        }
        // We need temporaries for intermediate doubles
        var prev = new RetroSharp.Generation.Intermediate.Model.Placeholder();
        // prev = source + source
        yield return new Add(prev, source, source);
        for (int i = 2; i <= k; i++)
        {
            if (i == k)
            {
                // target = prev + prev
                yield return new Add(accumTarget, prev, prev);
            }
            else
            {
                var next = new RetroSharp.Generation.Intermediate.Model.Placeholder();
                yield return new Add(next, prev, prev);
                prev = next;
            }
        }
    }
}
