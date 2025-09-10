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

public class DebugStackLayoutTests
{
    [Fact]
    public void Debug_stack_layout_at_IX_plus_4_read()
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

        // Find the PC for "LD L, (IX+4)" instruction in function f
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
        
        // Find the LD L, (IX+4) instruction (first parameter with new layout)
        var loadLPc = (ushort)dbg.Where(d => (d.LineText?.Contains("LD L, (IX+4)") ?? false))
                                 .Select(d => d.ProgramCounter)
                                 .DefaultIfEmpty(0).First();
        
        Assert.NotEqual((ushort)0, loadLPc);
        
        // Run until we reach the LD L, (IX+4) instruction
        const int MaxSteps = 20000;
        bool atLoad = false;
        for (int i = 0; i < MaxSteps; i++)
        {
            if (cpu.Registers.PC == loadLPc)
            {
                atLoad = true;
                break;
            }
            cpu.ExecuteNextInstruction();
        }
        Assert.True(atLoad, $"PC never reached LD L, (IX+4). Current PC=0x{cpu.Registers.PC:X4}");
        
        // At this point, examine the memory layout around IX
        var ix = (ushort)((cpu.Registers.IXH << 8) | (cpu.Registers.IXL & 0xFF));
        
        Console.WriteLine($"At LD L, (IX+4): IX = 0x{ix:X4}");
        for (int offset = -8; offset <= 8; offset++)
        {
            var addr = (ushort)(ix + offset);
            var value = cpu.Memory[addr];
            Console.WriteLine($"IX{offset:+#;-#;+0} (0x{addr:X4}) = 0x{value:X2} ({value})");
        }
        
        // The parameter (42 decimal = 0x2A) should be at IX+4 (low byte) and IX+5 (high byte)
        var paramLow = cpu.Memory[(ushort)(ix + 4)];
        var paramHigh = cpu.Memory[(ushort)(ix + 5)];
        var paramValue = paramHigh << 8 | paramLow;
        
        Assert.Equal(42, paramValue); // This should now pass with correct offset
    }
}
