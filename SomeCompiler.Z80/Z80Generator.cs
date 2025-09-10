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

                // Detect parameter label marker
                var paramLabel = body.OfType<SomeCompiler.Generation.Intermediate.Model.Codes.LocalLabel>()
                    .Select(l => l.Name)
                    .FirstOrDefault(n => n.StartsWith(lbl.Name + "::__params__:"));
                var paramNames = new List<string>();
                if (paramLabel != null)
                {
                    var idx = paramLabel.IndexOf("::__params__:", StringComparison.Ordinal);
                    if (idx >= 0)
                    {
                        var list = paramLabel[(idx + "::__params__:".Length)..];
                        if (!string.IsNullOrWhiteSpace(list))
                            paramNames = list.Split(',').Select(s => s.Trim()).Where(s => s.Length > 0).ToList();
                    }
                    // Remove marker from body so it doesn't render
                    body.RemoveAll(c => c is SomeCompiler.Generation.Intermediate.Model.Codes.LocalLabel ll && ll.Name == paramLabel);
                }

                // Build per-function reference set
                var refs = body.SelectMany(c => c.GetReferences()).Distinct().ToList();
                var mapNames = BuildNames(refs);

                // Assign offsets: parameters on stack at positive IX offsets, locals at negative IX offsets
                var table = new Dictionary<CodeGeneration.Model.Classes.Reference, MetaData>();
                int localIndex = 0;
                foreach (var r in refs)
                {
                    var name = mapNames[r];
                    if (r is SomeCompiler.Generation.Intermediate.Model.NamedReference nr)
                    {
                        // expected form func::name
                        if (nr.Value.StartsWith(lbl.Name + "::"))
                        {
                            var shortName = nr.Value.Substring(lbl.Name.Length + 2);
                            var pIndex = paramNames.FindIndex(p => p == shortName);
                            if (pIndex >= 0)
                            {
                                // EXPERIMENTAL: Try IX+2 - maybe params are right after caller_old_IX
                                var paramBaseOffset = 2 + (pIndex * 2);
                                var lowOffset = paramBaseOffset;
                                var highOffset = paramBaseOffset + 1;
                                table[r] = new MetaData(name, lowOffset, highOffset);
                                continue;
                            }
                        }
                    }
                    // Local: emulator layout low byte at higher address, high at lower
                    var high = -((localIndex + 1) * 2);
                    var low = high + 1;
                    table[r] = new MetaData(name, low, high);
                    localIndex++;
                }
                var frameSize = localIndex * 2;

                var op = new OpCodeEmitter(table);
                var emitter = new IntermediateEmitter(op, paramNames.Count, frameSize);
                var gen = new Z80AssemblyGenerator(emitter);

                // Emit label
                lines.Add($"{lbl.Name}:");

                // Get first parameter reference for prologue
                CodeGeneration.Model.Classes.Reference? firstParamRef = null;
                if (paramNames.Count >= 1)
                {
                    var firstParamName = paramNames[0];
                    var fullName = $"{lbl.Name}::{firstParamName}";
                    firstParamRef = refs.FirstOrDefault(r => r is SomeCompiler.Generation.Intermediate.Model.NamedReference nr && nr.Value == fullName);
                }

                // Extract parameter passing instructions that come before CALL instructions
                var (prePrologueInstructions, mainBody) = ExtractParameterInstructions(body);
                
                // Emit pre-prologue parameter instructions (before HL is modified)
                foreach (var c in prePrologueInstructions)
                {
                    lines.AddRange(gen.Generate(c));
                }

                // Prologue: different strategy based on parameters
                lines.AddRange(FunctionPrologue(frameSize, paramNames.Count, op));

                // Emit remaining body
                foreach (var c in mainBody)
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

    private static (List<SomeCompiler.Generation.Intermediate.Model.Codes.Code> prePrologue, List<SomeCompiler.Generation.Intermediate.Model.Codes.Code> mainBody) ExtractParameterInstructions(List<SomeCompiler.Generation.Intermediate.Model.Codes.Code> body)
    {
        var prePrologue = new List<SomeCompiler.Generation.Intermediate.Model.Codes.Code>();
        var mainBody = new List<SomeCompiler.Generation.Intermediate.Model.Codes.Code>();
        
        var i = 0;
        while (i < body.Count)
        {
            // Look for sequences of Param/ParamConst followed by Call
            if (IsParameterInstruction(body[i]))
            {
                var paramStart = i;
                // Collect consecutive parameter instructions
                while (i < body.Count && IsParameterInstruction(body[i]))
                {
                    i++;
                }
                
                // Check if followed by Call
                if (i < body.Count && body[i] is SomeCompiler.Generation.Intermediate.Model.Codes.Call)
                {
                    // Move param instructions to pre-prologue
                    for (var j = paramStart; j < i; j++)
                    {
                        prePrologue.Add(body[j]);
                    }
                    // Continue with Call and subsequent instructions in main body
                    while (i < body.Count)
                    {
                        mainBody.Add(body[i]);
                        i++;
                    }
                }
                else
                {
                    // No Call follows, keep in main body
                    for (var j = paramStart; j < i; j++)
                    {
                        mainBody.Add(body[j]);
                    }
                }
            }
            else
            {
                mainBody.Add(body[i]);
                i++;
            }
        }
        
        return (prePrologue, mainBody);
    }
    
    private static bool IsParameterInstruction(SomeCompiler.Generation.Intermediate.Model.Codes.Code code)
    {
        return code is SomeCompiler.Generation.Intermediate.Model.Codes.Param or SomeCompiler.Generation.Intermediate.Model.Codes.ParamConst;
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

    private static IEnumerable<string> FunctionPrologue(int frameSize, int paramCount, OpCodeEmitter op)
    {
        if (paramCount > 0)
        {
            // ACADEMIC CONVENTION: Callee with parameters does NOT touch IX
            // Use caller's IX directly to access parameters at IX+4, IX+6, etc.
            // Only adjust SP for local variables if needed
            if (frameSize > 0)
            {
                yield return $"\tLD HL, {-frameSize}";
                yield return "\tADD HL, SP";
                yield return "\tLD SP, HL";
            }
            // IX remains pointing to caller's frame
            // Layout: IX+0..1 -> [caller_old_IX], IX+2..3 -> [ret_addr], IX+4+ -> [params]
        }
        else
        {
            // Standard prologue for functions without parameters
            // Fixed: IX should point to where SP was BEFORE the setup  
            yield return "\tPUSH IX";     // [1] Save old IX
            yield return "\tLD HL, 2";   // [2] Account for PUSH IX (2 bytes)
            yield return "\tADD HL, SP";  // [3] HL = SP + 2 (before PUSH IX)
            yield return "\tPUSH HL";     // [4] Push corrected SP
            yield return "\tPOP IX";      // [5] IX = corrected SP
            
            if (frameSize > 0)
            {
                yield return $"\tLD HL, {-frameSize}";
                yield return "\tADD HL, SP";
                yield return "\tLD SP, HL";
            }
        }
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
