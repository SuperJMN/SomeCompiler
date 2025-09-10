using CSharpFunctionalExtensions;
using SomeCompiler.Generation.Intermediate;
using SomeCompiler.Z80.Core;
using Zafiro.Core.Mixins;

namespace SomeCompiler.Z80;

public class Z80Generator
{
    public Result<GeneratedProgram> Generate(SomeCompiler.Generation.Intermediate.Model.IntermediateCodeProgram program)
    {
        var codes = program.Cast<SomeCompiler.Generation.Intermediate.Model.Codes.Code>().ToList();
        var lines = new List<string>();

        // Partition program by functions starting at Label
        var i = 0;
        while (i < codes.Count)
        {
            if (codes[i] is SomeCompiler.Generation.Intermediate.Model.Codes.Label lbl)
            {
                // Collect function body until next Label or end
                var start = i;
                i++;
                var body = new List<SomeCompiler.Generation.Intermediate.Model.Codes.Code>();
                while (i < codes.Count && codes[i] is not SomeCompiler.Generation.Intermediate.Model.Codes.Label)
                {
                    body.Add(codes[i]);
                    i++;
                }

                // Build per-function reference set
                var refs = body.SelectMany(c => c.GetReferences()).Distinct().ToList();
                var mapNames = BuildNames(refs);
                var table = refs
                    .Select((r, idx) => new { r, idx })
                    .ToDictionary(x => x.r, x => new MetaData(mapNames[x.r], -((x.idx + 1) * 2))); // locals at negative offsets: -2, -4, ...
                var frameSize = refs.Count * 2;

                var op = new OpCodeEmitter(table);
                var emitter = new IntermediateEmitter(op);
                var gen = new Z80AssemblyGenerator(emitter);

                // Emit label
                lines.Add($"{lbl.Name}:");

                // Prologue
                lines.AddRange(FunctionPrologue(frameSize));

                // Emit body
                foreach (var c in body)
                {
                    lines.AddRange(gen.Generate(c));
                }
            }
            else
            {
                // Skip anything before the first label (shouldn't happen in current generator)
                i++;
            }
        }

        // Append shared algorithms
        lines.AddRange(MultiplyAlgorithm());

        var asm = lines.JoinWithLines();
        return new GeneratedProgram(asm);
    }

    private static Dictionary<CodeGeneration.Model.Classes.Reference, string> BuildNames(List<CodeGeneration.Model.Classes.Reference> refs)
    {
        var dict = new Dictionary<CodeGeneration.Model.Classes.Reference, string>();
        int t = 1;
        foreach (var r in refs)
        {
            switch (r)
            {
                case SomeCompiler.Generation.Intermediate.Model.NamedReference nr:
                    dict[r] = nr.Value;
                    break;
                default:
                    dict[r] = $"T{t++}";
                    break;
            }
        }
        return dict;
    }

    private static IEnumerable<string> FunctionPrologue(int frameSize)
    {
        // Establish IX as frame pointer and allocate frameSize bytes: SP = SP - frameSize
        yield return "\tPUSH IX";
        yield return "\tLD IX, 0";
        yield return "\tADD IX, SP";
        yield return $"\tLD HL, {-frameSize}";
        yield return "\tADD HL, SP";
        yield return "\tLD SP, HL";
    }

    public static IEnumerable<string> MultiplyAlgorithm()
    {
        yield return @"MUL16:
        LD      A,C             ; MULTIPLIER LOW PLACED IN A
        LD      C,B             ; MULTIPLIER HIGH PLACED IN C
        LD      B,$16           ; COUNTER (16 BITS)
        LD      HL,0            ;
MULT:
        SRL     C               ; RIGHT SHIFT MULTIPLIER HIGH
        RRA                     ; ROTATE RIGHT MULTIPLIER LOW
        JR      NC,NOADD        ; TEST CARRY
        ADD     HL,DE           ; ADD MULTIPLICAND TO RESULT
NOADD:
        EX      DE,HL
        ADD     HL,HL           ; SHIFT MULTIPLICAND LEFT
        EX      DE,HL           ;
        DJNZ    MULT            ;
        RET";
    }
}
