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

public class DebugArgDumpTests
{
    [Fact]
    public void Dump_stack_arg_at_f_entry_and_at_LD_L_IX_plus_4()
    {
        var src = @"int f(int n){ return n; } int main(){ return f(3); }";

        // Parse → Semantic → IR → ASM
        var parseResult = new SomeParser().Parse(src);
        Assert.True(parseResult.IsSuccess, parseResult.IsFailure ? parseResult.Error : "");
        var analyzed = new SemanticAnalyzer().Analyze(parseResult.Value);
        var root = (SemanticNode)analyzed.Node;
        var allErrors = root.AllErrors.ToList();
        Assert.True(allErrors.Count == 0, string.Join("\n", allErrors));
        var programNode = (ProgramNode)analyzed.Node;

        var ir = new V2IntermediateCodeGenerator().Generate(programNode);
        var asmResult = new Z80Generator().Generate(ir);
        Assert.True(asmResult.IsSuccess, asmResult.IsFailure ? asmResult.Error : "");
        var asm = asmResult.Value.Assembly;

        var assembler = new Z80Assembler();
        var assembled = assembler.Assemble(asm);
        Assert.True(assembled.IsSuccess, assembled.IsFailure ? assembled.Error : "");
        var dbg = assembled.Value.DebugInfo.ToList();

        // Locate store of first param in f: "LD (IX-3), L"
        var storeLowPc = (ushort)dbg.Where(d => (d.LineText?.Contains("LD (IX-3), L") ?? false))
                                     .Select(d => d.ProgramCounter)
                                     .DefaultIfEmpty(0).First();
        Assert.NotEqual((ushort)0, storeLowPc);
        // Find the CALL f site in main (allow comment suffix)
        var callFPc = (ushort)dbg.Where(d => (d.LineText?.Contains("CALL f") ?? false))
                                  .Select(d => d.ProgramCounter)
                                  .DefaultIfEmpty(0).First();
        Assert.NotEqual((ushort)0, callFPc);

        // Build reliable entry PC using the shared helper (it locates 'main:')
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

        // Run until just before CALL f in main
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

        // At this point (before CALL), HL should already be 3 in our calling convention
        var hlBeforeCall = (cpu.Registers.H << 8) | (cpu.Registers.L & 0xFF);
        Assert.Equal(3, hlBeforeCall);

        // Execute CALL f, then continue until LD (IX-3), L (shadow store) to inspect HL just before it executes
        cpu.ExecuteNextInstruction(); // CALL f
        bool atShadow = false;
        for (int i = 0; i < MaxSteps; i++)
        {
            if (cpu.Registers.PC == storeLowPc)
            {
                atShadow = true;
                break;
            }
            cpu.ExecuteNextInstruction();
        }
        Assert.True(atShadow, $"PC never reached param store. Current PC=0x{cpu.Registers.PC:X4}");
        var hlAtShadow = (cpu.Registers.H << 8) | (cpu.Registers.L & 0xFF);
        Assert.Equal(3, hlAtShadow);
    }
}
