using CSharpFunctionalExtensions;
using RetroSharp.Generation.Intermediate;
using RetroSharp.Z80.Core;
using Zafiro.Core.Mixins;

namespace RetroSharp.Z80;

public class Z80Generator
{
    public Result<GeneratedProgram> Generate(RetroSharp.Generation.Intermediate.Model.IntermediateCodeProgram program)
    {
        var codes = program.Cast<RetroSharp.Generation.Intermediate.Model.Codes.Code>().ToList();
        var lines = new List<string>();

        // Partition program by functions starting at Label
        var i = 0;
        while (i < codes.Count)
        {
            if (codes[i] is RetroSharp.Generation.Intermediate.Model.Codes.Label lbl)
            {
                // Collect function body until next Label or end
                var start = i;
                i++;
                var body = new List<RetroSharp.Generation.Intermediate.Model.Codes.Code>();
                while (i < codes.Count && codes[i] is not RetroSharp.Generation.Intermediate.Model.Codes.Label)
                {
                    body.Add(codes[i]);
                    i++;
                }

                // Detect parameter label marker
                var paramLabel = body.OfType<RetroSharp.Generation.Intermediate.Model.Codes.LocalLabel>()
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
                    body.RemoveAll(c => c is RetroSharp.Generation.Intermediate.Model.Codes.LocalLabel ll && ll.Name == paramLabel);
                }

                // Build per-function reference set
                var allRefs = body.SelectMany(c => c.GetReferences()).Distinct().ToList();
                
                // Optimization: Filter out single-use temporaries that are only used in return statements
                var refs = FilterOutSingleUseReturnTemporaries(body, allRefs);
                
                var mapNames = BuildNames(refs);

                // Assign offsets: parameters on stack at positive IX offsets, locals at negative IX offsets
                var table = new Dictionary<CodeGeneration.Model.Classes.Reference, MetaData>();
                int localIndex = 0;
                foreach (var r in refs)
                {
                    var name = mapNames[r];
                    if (r is RetroSharp.Generation.Intermediate.Model.NamedReference nr)
                    {
                        // expected form func::name
                        if (nr.Value.StartsWith(lbl.Name + "::"))
                        {
                            var shortName = nr.Value.Substring(lbl.Name.Length + 2);
                            var pIndex = paramNames.FindIndex(p => p == shortName);
                            if (pIndex >= 0)
                            {
                                // Standard Z80 frame layout:
                                // (ix+0,ix+1) : old IX
                                // (ix+2,ix+3) : return address  
                                // (ix+4,...)  : arguments (param0 at ix+4, param1 at ix+6, etc)
                                var paramBaseOffset = 4 + (pIndex * 2);
                                var lowOffset = paramBaseOffset;     // Low byte
                                var highOffset = paramBaseOffset + 1; // High byte
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
                
                // Parameter detection completed
                
                var emitter = new IntermediateEmitter(op, paramNames.Count, frameSize);
                var gen = new Z80AssemblyGenerator(emitter);

                // Emit label
                lines.Add($"{lbl.Name}:");


                // Generate prologue
                lines.AddRange(FunctionPrologue(frameSize, paramNames.Count, op));
                
                // Emit function body
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

        // Only append shared algorithms if they are used
        bool hasMultiplication = codes.OfType<RetroSharp.Generation.Intermediate.Model.Codes.Multiply>().Any();
        if (hasMultiplication)
        {
            lines.AddRange(MultiplyAlgorithm());
        }

        var asm = lines.JoinWithLines();
        return new GeneratedProgram(asm);
    }

    
    /// <summary>
    /// Optimization: Filter out temporaries that are only used for "assign constant; return temp" patterns.
    /// This avoids materializing values that can stay in HL register.
    /// </summary>
    private static List<CodeGeneration.Model.Classes.Reference> FilterOutSingleUseReturnTemporaries(
        List<RetroSharp.Generation.Intermediate.Model.Codes.Code> body, 
        List<CodeGeneration.Model.Classes.Reference> allRefs)
    {
        var filteredRefs = new List<CodeGeneration.Model.Classes.Reference>();
        
        foreach (var reference in allRefs)
        {
            // Simple optimization: if a reference is used exactly twice - once in AssignConstant as target,
            // once in Return as source - then we can keep the value in HL and skip stack materialization
            var assignConstantUsage = body.OfType<RetroSharp.Generation.Intermediate.Model.Codes.AssignConstant>()
                .FirstOrDefault(ac => ac.Target.Equals(reference));
            
            var returnUsage = body.OfType<RetroSharp.Generation.Intermediate.Model.Codes.Return>()
                .FirstOrDefault(ret => ret.Reference.Equals(reference));
            
            var totalUsages = body.SelectMany(c => c.GetReferences()).Count(r => r.Equals(reference));
            
            // Skip materialization if: exactly 2 usages, one assign constant, one return
            if (totalUsages == 2 && assignConstantUsage != null && returnUsage != null)
            {
                continue; // Don't materialize this reference
            }
            
            filteredRefs.Add(reference);
        }
        
        return filteredRefs;
    }

    private static Dictionary<CodeGeneration.Model.Classes.Reference, string> BuildNames(List<CodeGeneration.Model.Classes.Reference> refs)
    {
        var dict = new Dictionary<CodeGeneration.Model.Classes.Reference, string>();
        int t = 1;
        foreach (var r in refs)
        {
            switch (r)
            {
                case RetroSharp.Generation.Intermediate.Model.NamedReference nr:
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
        bool needsFrame = paramCount > 0 || frameSize > 0 || op.HasAnyReferences();
        
        if (needsFrame)
        {
            // Standard Z80 calling convention prologue
            // On entry: [old_stack, arg0, arg1, ..., return_addr] <- SP (for functions with parameters)
            yield return "\tPUSH IX";          // Save old frame pointer
            yield return "\tLD HL, 0";
            yield return "\tADD HL, SP";       // HL = current SP
            yield return "\tPUSH HL";          // Push HL onto stack
            yield return "\tPOP IX";           // IX = value from stack = HL
            
            // Reserve space for locals if needed
            if (frameSize > 0)
            {
                yield return $"\tLD HL, {-frameSize}";
                yield return "\tADD HL, SP";
                yield return "\tLD SP, HL";
            }
            
            // IX frame layout:
            // (ix+0,ix+1) : old IX
            // (ix+2,ix+3) : return address  
            // (ix+4,...)  : arguments (if any)
        }
        // Simple functions with no frame needs get no prologue
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
