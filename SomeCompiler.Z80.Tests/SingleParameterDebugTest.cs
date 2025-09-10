using System;
using System.Linq;
using SomeCompiler.Parser;
using SomeCompiler.SemanticAnalysis;
using SomeCompiler.Generation.Intermediate;
using SomeCompiler.Z80;
using Sixty502DotNet;
using Konamiman.Z80dotNet;
using Xunit;

namespace SomeCompiler.Z80.Tests;

public class SingleParameterDebugTest
{
    [Fact]
    public void Debug_single_parameter_stack_layout()
    {
        var src = @"int f(int n) { return n; } int main() { return f(42); }";

        var (bin, entryPc) = Support.Z80E2E.BuildBinaryAndEntryPc(src);
        
        // Setup CPU
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

        // Get debug info
        var parseResult = new SomeParser().Parse(src);
        var analyzed = new SemanticAnalyzer().Analyze(parseResult.Value);
        var root = (SemanticNode)analyzed.Node;
        var programNode = (ProgramNode)analyzed.Node;
        var ir = new V2IntermediateCodeGenerator().Generate(programNode);
        var asmResult = new Z80Generator().Generate(ir);
        var asm = asmResult.Value.Assembly;
        
        var assembler = new Z80Assembler();
        var assembled = assembler.Assemble(asm);
        var dbg = assembled.Value.DebugInfo.ToList();
        
        Console.WriteLine("=== ASSEMBLY GENERATED ===");
        Console.WriteLine(asm);
        Console.WriteLine("=========================");
        
        // Find key instructions
        var pushHLPc = (ushort)dbg.Where(d => (d.LineText?.Contains("PUSH HL") ?? false) && !d.LineText.Contains("POP"))
                                 .Select(d => d.ProgramCounter)
                                 .DefaultIfEmpty(0).First();
        
        var callFPc = (ushort)dbg.Where(d => (d.LineText?.Contains("CALL f") ?? false))
                                 .Select(d => d.ProgramCounter)
                                 .DefaultIfEmpty(0).First();
                                 
        var ldIXPc = (ushort)dbg.Where(d => (d.LineText?.Contains("LD L, (IX+4)") ?? false))
                               .Select(d => d.ProgramCounter)
                               .DefaultIfEmpty(0).First();
        
        Console.WriteLine($"PUSH HL at PC: 0x{pushHLPc:X4}");
        Console.WriteLine($"CALL f at PC: 0x{callFPc:X4}");
        Console.WriteLine($"LD L,(IX+4) at PC: 0x{ldIXPc:X4}");
        
        // Run until PUSH HL
        for (int i = 0; i < 20000 && cpu.Registers.PC != pushHLPc; i++)
        {
            cpu.ExecuteNextInstruction();
        }
        
        Console.WriteLine($"Before PUSH HL: SP=0x{cpu.Registers.SP:X4}, HL=0x{(cpu.Registers.H << 8 | cpu.Registers.L):X4}");
        
        // Execute PUSH HL
        cpu.ExecuteNextInstruction();
        var spAfterPush = (ushort)cpu.Registers.SP;
        Console.WriteLine($"After PUSH HL: SP=0x{spAfterPush:X4}");
        Console.WriteLine($"Value at stack: 0x{cpu.Memory[spAfterPush] | (cpu.Memory[spAfterPush + 1] << 8):X4}");
        
        // Run until LD L,(IX+4)  
        for (int i = 0; i < 20000 && cpu.Registers.PC != ldIXPc; i++)
        {
            cpu.ExecuteNextInstruction();
        }
        
        var ix = (ushort)((cpu.Registers.IXH << 8) | (cpu.Registers.IXL & 0xFF));
        Console.WriteLine($"At LD L,(IX+4): IX=0x{ix:X4}, SP=0x{cpu.Registers.SP:X4}");
        
        // Show complete memory layout around IX
        Console.WriteLine("Memory around IX:");
        for (int offset = -4; offset <= 8; offset++)
        {
            var addr = (ushort)(ix + offset);
            var value = cpu.Memory[addr];
            Console.WriteLine($"IX{offset:+#;-#;+0} (0x{addr:X4}) = 0x{value:X2} ({value})");
        }
        
        // Also show where the parameter 42 actually is in memory
        Console.WriteLine($"Searching for value 42 (0x2A) in memory range 0xFE00-0xFF00:");
        for (ushort addr = 0xFE00; addr <= 0xFF00; addr++)
        {
            if (cpu.Memory[addr] == 0x2A) // 42 in hex
            {
                var nextByte = cpu.Memory[addr + 1];
                Console.WriteLine($"Found 0x2A at 0x{addr:X4}, next byte = 0x{nextByte:X2}");
            }
        }
        
        Assert.True(false, "This test is for debugging - check console output");
    }
}
