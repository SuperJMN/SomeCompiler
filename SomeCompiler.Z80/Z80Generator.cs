using CodeGeneration.Model.Classes;
using CSharpFunctionalExtensions;
using SomeCompiler.Generation.Intermediate;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Z80.Core;

namespace SomeCompiler.Z80;

public class Z80Generator
{
    public Result<GeneratedProgram> Generate(IntermediateCodeProgram program)
    {
        var getNames = GetNames(program);
        var addresses = GetMemAddresses(program);
        var table = getNames.Join(addresses, t => t.Item1, y => y.Key, (a, b) => new{ a.Item1, a.Item2, b.Value }).ToDictionary(x => x.Item1, x => new MetaData(x.Item2, x.Value));
        var generator = new Z80AssemblyGenerator(new IntermediateEmitter(new OpCodeEmitter(table)));

        var asm = program
            .Select(c => generator.Generate(c).JoinWithLines())
            .Concat(MultiplyAlgorithm())
            .JoinWithLines();
        
        return new GeneratedProgram(asm);
    }

    private static Dictionary<Reference, int> GetMemAddresses(IntermediateCodeProgram program)
    {
        return program.IndexedReferences().ToDictionary(t => t.Reference, t => t.Index * 2 + 0x70);
    }

    private IEnumerable<(Reference, string)> GetNames(IntermediateCodeProgram program)
    {
        var named = program.NamedReferences().Select(x => ((Reference) x, x.Value));
        var unnamed = program.UnnamedReferences().Select((x, i) => (x, $"T{i+1}"));
        return named.Concat(unnamed);
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