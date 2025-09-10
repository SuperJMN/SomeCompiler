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

public class FindParameterTests
{
    [Fact]
    public void Find_parameter_42_in_memory()
    {
        var src = @"int f(int n){ return n; } int main(){ return f(42); }";

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

        // Find the CALL f instruction in main
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
        
        // Find the CALL f instruction
        var callFPc = (ushort)dbg.Where(d => (d.LineText?.Contains("CALL f") ?? false))
                                 .Select(d => d.ProgramCounter)
                                 .DefaultIfEmpty(0).First();
        
        Assert.NotEqual((ushort)0, callFPc);
        
        // Run until we reach CALL f, then step into the function
        const int MaxSteps = 20000;
        bool atCall = false;
        for (int i = 0; i < MaxSteps; i++)
        {
            if (cpu.Registers.PC == callFPc)
            {
                atCall = true;
                break;
            }
            cpu.ExecuteNextInstruction();
        }
        Assert.True(atCall, $"PC never reached CALL f. Current PC=0x{cpu.Registers.PC:X4}");
        
        // At this point, the parameter should be on the stack just before the call
        var spBeforeCall = (ushort)cpu.Registers.SP;
        Console.WriteLine($"SP before CALL f: 0x{spBeforeCall:X4}");
        
        // Execute the CALL instruction
        cpu.ExecuteNextInstruction();
        
        var spAfterCall = (ushort)cpu.Registers.SP;
        Console.WriteLine($"SP after CALL f: 0x{spAfterCall:X4}");
        
        // Now search for the value 42 (0x2A) in memory around the stack area
        Console.WriteLine("Searching for parameter 42 (0x2A) in memory:");
        bool found42 = false;
        for (ushort addr = 0xFE00; addr <= 0xFF00; addr++)
        {
            var value = cpu.Memory[addr];
            if (value == 0x2A) // 42 in hex
            {
                var nextValue = cpu.Memory[addr + 1];
                Console.WriteLine($"Found 0x2A at address 0x{addr:X4}, next byte = 0x{nextValue:X2}");
                found42 = true;
            }
        }
        
        if (!found42)
        {
            Console.WriteLine("Parameter 42 not found in stack area!");
        }
        
        // Also check what's on the stack around SP
        Console.WriteLine($"Stack dump around SP (0x{spAfterCall:X4}):");
        for (int offset = -10; offset <= 10; offset++)
        {
            var addr = (ushort)(spAfterCall + offset);
            var value = cpu.Memory[addr];
            Console.WriteLine($"SP{offset:+#;-#;+0} (0x{addr:X4}) = 0x{value:X2} ({value})");
        }
        
        Assert.True(found42, "Parameter 42 should be found in memory");
    }
}
