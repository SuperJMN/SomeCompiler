using Konamiman.Z80dotNet;
using Sixty502DotNet;
using Xunit;
using Xunit.Abstractions;

namespace SomeCompiler.Z80.Tests;

public class StackConventionProbeTests
{
    private readonly ITestOutputHelper output;
    public StackConventionProbeTests(ITestOutputHelper output) => this.output = output;

    [Fact]
    public void Probe_all_pairs_and_report()
    {
        // Pairs to test: (lowOff, highOff)
        var candidates = new (int low, int high, string name)[]
        {
            (4,5, "+4/+5"), (5,4, "+5/+4"),
            (2,3, "+2/+3"), (3,2, "+3/+2"),
            (6,7, "+6/+7"), (7,6, "+7/+6"),
            (8,9, "+8/+9"), (9,8, "+9/+8"),
        };

        var results = new List<(string name, int value)>();
        foreach (var (low, high, name) in candidates)
        {
            var value = RunProbe(low, high);
            results.Add((name, value));
        }

        foreach (var r in results)
        {
            var line = $"Pair {r.name} -> HL={r.value}";
            output.WriteLine(line);
            Console.WriteLine(line);
        }

        Assert.Contains(results, r => r.value == 3);
    }
    
    [Fact]
    public void Probe_with_42_using_compiler_pattern()
    {
        // Test different offsets to find where 42 really is
        var candidates = new (int low, int high, string name)[]
        {
            (4,5, "+4/+5"), (5,4, "+5/+4"),
            (2,3, "+2/+3"), (3,2, "+3/+2"),
            (6,7, "+6/+7"), (7,6, "+7/+6"),
        };

        foreach (var (low, high, name) in candidates)
        {
            var value = RunProbeValue42(low, high);
            var line = $"Probe 42 at {name}: {value}";
            output.WriteLine(line);
            Console.WriteLine(line);
            if (value == 42)
            {
                Assert.Equal(42, value); // Success!
                return;
            }
        }
        
        Assert.True(false, "Value 42 not found at any expected offset");
    }
    
    [Fact]
    public void Probe_exact_compiler_layout_all_offsets()
    {
        // Test many offsets to find where 42 really is with the exact compiler pattern
        var results = new List<(int offset, int value)>();
        
        for (int offset = 0; offset <= 20; offset++)
        {
            try {
                var value = RunCompilerPatternProbe(offset);
                results.Add((offset, value));
                if (value == 42) {
                    var line = $"*** FOUND 42 at offset {offset} ***";
                    output.WriteLine(line);
                    Console.WriteLine(line);
                }
            } catch (Exception ex) {
                results.Add((offset, -1)); // Error marker
                output.WriteLine($"Offset {offset}: ERROR - {ex.Message}");
            }
        }
        
        foreach (var (offset, value) in results)
        {
            var line = $"Offset {offset}: {value}";
            output.WriteLine(line);
        }
        
        Assert.Contains(results, r => r.value == 42);
    }
    
    private static int RunProbeValue42(int lowOff, int highOff)
    {
        // Generate the exact same pattern as our compiler for passing value 42
        string asm = $@"main:
	LD HL, 42
	PUSH HL
	PUSH IX
	LD HL, 0
	ADD HL, SP
	PUSH HL
	POP IX
	LD HL, -2
	ADD HL, SP
	LD SP, HL
	CALL f
	LD D, H
	LD E, L
	LD HL, 2
	ADD HL, SP
	LD SP, HL
	LD H, D
	LD L, E
	LD SP, IX
	POP IX
	RET
f:
	PUSH IX
	LD HL, 0
	ADD HL, SP
	PUSH HL
	POP IX
	LD HL, -2
	ADD HL, SP
	LD SP, HL
	LD L, (IX+{lowOff})
	LD H, (IX+{highOff})
	LD SP, IX
	POP IX
	RET";
        
        var assembler = new Z80Assembler();
        var assembled = assembler.Assemble(asm);
        if (assembled.IsFailure) throw new Exception(assembled.Error);
        var bin = assembled.Value.ProgramBinary;
        var entryPc = (ushort)assembled.Value.DebugInfo
            .Where(d => (d.LineText?.Trim() ?? string.Empty) == "main:")
            .Select(d => d.ProgramCounter)
            .DefaultIfEmpty(0)
            .First();

        var cpu = new Z80Processor();
        cpu.Reset();
        cpu.Memory.SetContents(0, bin);
        const ushort haltAddr = 0xF000;
        cpu.Memory[haltAddr] = 0x76; // HALT
        const ushort s0 = 0xFF00;
        cpu.Memory[s0] = (byte)(haltAddr & 0xFF);
        cpu.Memory[s0 + 1] = (byte)(haltAddr >> 8);
        cpu.Registers.SP = unchecked((short)s0);
        cpu.Registers.PC = entryPc;

        const int MaxSteps = 1000; // Reduced timeout for debugging
        int stepCount = 0;
        try {
            for (stepCount = 0; stepCount < MaxSteps && !cpu.IsHalted; stepCount++) 
                cpu.ExecuteNextInstruction();
        } catch (Exception ex) {
            throw new Exception($"CPU error at step {stepCount}: {ex.Message}", ex);
        }
        if (!cpu.IsHalted) 
            throw new TimeoutException($"CPU did not HALT after {stepCount} steps. PC={cpu.Registers.PC:X4}, SP={cpu.Registers.SP:X4}");
        return (cpu.Registers.H << 8) | (cpu.Registers.L & 0xFF);
    }
    
    private static int RunCompilerPatternProbe(int offset)
    {
        // Use the exact same assembly pattern our compiler generates
        string asm = $@"main:
	LD HL, 42
	PUSH HL
	PUSH IX
	LD HL, 0
	ADD HL, SP
	PUSH HL
	POP IX
	LD HL, -2
	ADD HL, SP
	LD SP, HL
	CALL f
	LD SP, IX
	POP IX
	RET
f:
	PUSH IX
	LD HL, 0
	ADD HL, SP
	PUSH HL
	POP IX
	LD HL, -2
	ADD HL, SP
	LD SP, HL
	LD L, (IX+{offset})
	LD H, (IX+{offset + 1})
	LD SP, IX
	POP IX
	RET";
        
        var assembler = new Z80Assembler();
        var assembled = assembler.Assemble(asm);
        if (assembled.IsFailure) throw new Exception($"ASM Error: {assembled.Error}");
        var bin = assembled.Value.ProgramBinary;
        var entryPc = (ushort)assembled.Value.DebugInfo
            .Where(d => (d.LineText?.Trim() ?? string.Empty) == "main:")
            .Select(d => d.ProgramCounter)
            .DefaultIfEmpty(0)
            .First();

        var cpu = new Z80Processor();
        cpu.Reset();
        cpu.Memory.SetContents(0, bin);
        const ushort haltAddr = 0xF000;
        cpu.Memory[haltAddr] = 0x76; // HALT
        const ushort s0 = 0xFF00;
        cpu.Memory[s0] = (byte)(haltAddr & 0xFF);
        cpu.Memory[s0 + 1] = (byte)(haltAddr >> 8);
        cpu.Registers.SP = unchecked((short)s0);
        cpu.Registers.PC = entryPc;

        const int MaxSteps = 1000;
        int stepCount = 0;
        for (stepCount = 0; stepCount < MaxSteps && !cpu.IsHalted; stepCount++) 
            cpu.ExecuteNextInstruction();
        
        if (!cpu.IsHalted) 
            throw new TimeoutException($"Did not HALT after {stepCount} steps");
        
        return (cpu.Registers.H << 8) | (cpu.Registers.L & 0xFF);
    }

    private static int RunProbe(int lowOff, int highOff)
    {
        // Assemble a tiny program:
        // main: prologue with locals, HL=3 -> store temp -> reload -> PUSH HL; CALL f; clean args; RET
        // f: prologue with small frame; HL = (IX+lowOff/highOff); RET
        string asm = $@"main:
	PUSH IX
	LD IX, 0
	ADD IX, SP
	; reserve 4 bytes for locals like generated code
	LD HL, -4
	ADD HL, SP
	LD SP, HL
	; put 3 into a temp and reload (like generated main)
	LD HL, 3
	LD (IX-1), L
	LD (IX-2), H
	LD L, (IX-1)
	LD H, (IX-2)
	PUSH HL
	CALL f
	; preserve HL while cleaning 2 bytes of args from stack
	LD D, H
	LD E, L
	LD HL, 2
	ADD HL, SP
	LD SP, HL
	LD H, D
	LD L, E
	LD SP, IX
	POP IX
	RET
f:
	PUSH IX
	LD IX, 0
	ADD IX, SP
	; reserve small local frame (2 bytes)
	LD D, H
	LD E, L
	LD HL, -2
	ADD HL, SP
	LD SP, HL
	LD H, D
	LD L, E
	; read argument
	LD L, (IX+{lowOff})
	LD H, (IX+{highOff})
	LD SP, IX
	POP IX
	RET";

        var assembler = new Z80Assembler();
        var assembled = assembler.Assemble(asm);
        if (assembled.IsFailure) throw new Exception(assembled.Error);
        var bin = assembled.Value.ProgramBinary;
        var entryPc = (ushort)assembled.Value.DebugInfo
            .Where(d => (d.LineText?.Trim() ?? string.Empty) == "main:")
            .Select(d => d.ProgramCounter)
            .DefaultIfEmpty(0)
            .First();

        var cpu = new Z80Processor();
        cpu.Reset();
        cpu.Memory.SetContents(0, bin);
        const ushort haltAddr = 0xF000;
        cpu.Memory[haltAddr] = 0x76; // HALT
        const ushort s0 = 0xFF00;
        cpu.Memory[s0] = (byte)(haltAddr & 0xFF);
        cpu.Memory[s0 + 1] = (byte)(haltAddr >> 8);
        cpu.Registers.SP = unchecked((short)s0);
        cpu.Registers.PC = entryPc;

        const int MaxSteps = 10000;
        for (int i = 0; i < MaxSteps && !cpu.IsHalted; i++) cpu.ExecuteNextInstruction();
        if (!cpu.IsHalted) throw new TimeoutException("CPU did not HALT");
        return (cpu.Registers.H << 8) | (cpu.Registers.L & 0xFF);
    }
}
