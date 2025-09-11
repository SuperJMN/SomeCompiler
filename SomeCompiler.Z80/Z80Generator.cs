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
                    if (r is SomeCompiler.Generation.Intermediate.Model.NamedReference nr)
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
                
                // Prologue: different strategy based on parameters and whether we're a caller
                bool isCaller = prePrologueInstructions.Count > 0;
                lines.AddRange(FunctionPrologue(frameSize, paramNames.Count, op, isCaller));
                
                // Emit pre-prologue parameter instructions (after setting up the frame)
                foreach (var c in prePrologueInstructions)
                {
                    lines.AddRange(gen.Generate(c));
                }

                // The callee will set up its own frame in the standard prologue

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

        // Only append shared algorithms if they are used
        bool hasMultiplication = codes.OfType<SomeCompiler.Generation.Intermediate.Model.Codes.Multiply>().Any();
        if (hasMultiplication)
        {
            lines.AddRange(MultiplyAlgorithm());
        }

        var asm = lines.JoinWithLines();
        return new GeneratedProgram(asm);
    }

    private static (List<SomeCompiler.Generation.Intermediate.Model.Codes.Code> prePrologue, List<SomeCompiler.Generation.Intermediate.Model.Codes.Code> mainBody) ExtractParameterInstructions(List<SomeCompiler.Generation.Intermediate.Model.Codes.Code> body)
    {
        var prePrologue = new List<SomeCompiler.Generation.Intermediate.Model.Codes.Code>();
        var mainBody = new List<SomeCompiler.Generation.Intermediate.Model.Codes.Code>();
        
        // TEMPORARILY DISABLE parameter extraction to fix factorial bug
        // The issue is that param instructions often depend on calculations in the function body,
        // but ExtractParameterInstructions moves them to pre-prologue where those calculations
        // haven't happened yet, causing wrong values to be pushed.
        // TODO: Make this smarter to only extract truly independent parameter instructions
        
        // For now, keep everything in main body
        mainBody.AddRange(body);
        
        return (prePrologue, mainBody);
    }
    
    private static bool IsParameterInstruction(SomeCompiler.Generation.Intermediate.Model.Codes.Code code)
    {
        return code is SomeCompiler.Generation.Intermediate.Model.Codes.Param or SomeCompiler.Generation.Intermediate.Model.Codes.ParamConst;
    }
    
    private static List<CodeGeneration.Model.Classes.Reference> FilterOutSingleUseReturnTemporaries(
        List<SomeCompiler.Generation.Intermediate.Model.Codes.Code> body, 
        List<CodeGeneration.Model.Classes.Reference> allRefs)
    {
        var filteredRefs = new List<CodeGeneration.Model.Classes.Reference>();
        
        foreach (var reference in allRefs)
        {
            // Count how many times this reference is used across all instructions
            var usageCount = 0;
            var isOnlyUsedInReturn = false;
            
            foreach (var code in body)
            {
                var codeRefs = code.GetReferences();
                if (codeRefs.Contains(reference))
                {
                    usageCount++;
                    // Check if this usage is in a return statement
                    if (code is SomeCompiler.Generation.Intermediate.Model.Codes.Return ret && ret.Reference.Equals(reference))
                    {
                        isOnlyUsedInReturn = true;
                    }
                    else
                    {
                        isOnlyUsedInReturn = false;
                    }
                }
            }
            
            // If reference is used exactly twice (once in assign, once in return), and the return is the only non-assign usage,
            // then we can optimize it out
            if (usageCount == 2 && isOnlyUsedInReturn)
            {
                // Check if the other usage is an AssignConstant where this reference is the target
                bool hasAssignConstantUsage = body.OfType<SomeCompiler.Generation.Intermediate.Model.Codes.AssignConstant>()
                    .Any(ac => ac.Target.Equals(reference));
                
                if (hasAssignConstantUsage)
                {
                    // Skip this reference - don't materialize it
                    continue;
                }
            }
            
            // Keep this reference
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


    private static IEnumerable<string> FunctionPrologue(int frameSize, int paramCount, OpCodeEmitter op, bool isCaller = false)
    {
        if (paramCount > 0)
        {
            // Standard Z80 calling convention prologue for functions with parameters
            // On entry: [old_stack, arg0, arg1, ..., return_addr] <- SP
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
            // Now IX points to the frame:
            // (ix+0,ix+1) : old IX
            // (ix+2,ix+3) : return address  
            // (ix+4,...)  : arguments
        }
        else if (frameSize > 0 || op.HasAnyReferences())
        {
            // Standard prologue for functions that need IX frame (have locals or use IX)
            yield return "\tPUSH IX";         // Save old IX
            yield return "\tLD HL, 0";
            yield return "\tADD HL, SP";      // HL = current SP
            yield return "\tPUSH HL";         // Push HL onto stack
            yield return "\tPOP IX";          // IX = value from stack = HL
            
            if (frameSize > 0)
            {
                yield return $"\tLD HL, {-frameSize}";
                yield return "\tADD HL, SP";
                yield return "\tLD SP, HL";
            }
        }
        else
        {
            // Simple functions with no locals and no IX usage: minimal prologue
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
