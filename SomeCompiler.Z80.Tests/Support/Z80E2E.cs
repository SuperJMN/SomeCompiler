using System;
using System.Linq;
using SomeCompiler.Parser;                    // SomeParser (Antlr v2)
using SomeCompiler.SemanticAnalysis;         // SemanticAnalyzer, ProgramNode
using SomeCompiler.Generation.Intermediate;  // V2IntermediateCodeGenerator
using SomeCompiler.Z80;                      // Z80Generator
using Sixty502DotNet;                        // Z80Assembler
using Konamiman.Z80dotNet;                   // Z80Processor

namespace SomeCompiler.Z80.Tests.Support;

public static class Z80E2E
{
    // One-liner para tests: compila y devuelve HL tras ejecutar main.
    public static int RunHL(string source, int maxSteps = 20000)
    {
        var (bin, entryPc) = BuildBinaryAndEntryPc(source);
        return ExecuteAndGetHL(bin, entryPc, maxSteps);
    }

    // Devuelve la CPU por si quieres inspeccionar registros/memoria.
    public static Z80Processor RunCpu(string source, int maxSteps = 20000)
    {
        var (bin, entryPc) = BuildBinaryAndEntryPc(source);
        return ExecuteCpu(bin, entryPc, maxSteps);
    }

    // Parser -> Semántico -> IR -> ASM -> Binario + entry PC
    public static (byte[] bin, ushort entryPc) BuildBinaryAndEntryPc(string source)
    {
        // Parse
        var parseResult = new SomeParser().Parse(source);
        if (parseResult.IsFailure)
            throw new InvalidOperationException(parseResult.Error);
        var programSyntax = parseResult.Value;

        // Semántico
        var analyzed = new SemanticAnalyzer().Analyze(programSyntax);
        var root = (SemanticNode)analyzed.Node;
        var allErrors = root.AllErrors.ToList();
        if (allErrors.Count > 0)
            throw new InvalidOperationException(string.Join("\n", allErrors));
        var programNode = (ProgramNode)analyzed.Node;

        // IR
        var ir = new V2IntermediateCodeGenerator().Generate(programNode);

        // ASM
        var asmResult = new Z80Generator().Generate(ir);
        if (asmResult.IsFailure)
            throw new InvalidOperationException(asmResult.Error);
        var asm = asmResult.Value.Assembly;

        // Ensamblar
        var assembler = new Z80Assembler();
        var assembled = assembler.Assemble(asm);
        if (assembled.IsFailure)
            throw new InvalidOperationException(assembled.Error);

        var bin = assembled.Value.ProgramBinary;

        // Entry = PC relativo a origen 0 de etiqueta "main:"
        var entryPc = (ushort)assembled.Value.DebugInfo
            .Where(d => (d.LineText?.Trim() ?? string.Empty) == "main:")
            .Select(d => d.ProgramCounter)
            .DefaultIfEmpty(0)
            .First();

        return (bin, entryPc);
    }

    // Ejecuta y devuelve HL (valor retornado por main)
    public static int ExecuteAndGetHL(byte[] bin, ushort entryPc, int maxSteps = 20000)
    {
        var cpu = ExecuteCpu(bin, entryPc, maxSteps);
        return (cpu.Registers.H << 8) | (cpu.Registers.L & 0xFF);
    }

    // Ejecuta paso a paso con retorno determinista (RET -> HALT)
    public static Z80Processor ExecuteCpu(byte[] bin, ushort entryPc, int maxSteps = 20000)
    {
        var cpu = new Z80Processor();
        cpu.Reset();

        // Cargar binario en origen 0 (mismo que Sixty502DotNet)
        cpu.Memory.SetContents(0, bin);

        // Preparar retorno a HALT: RET saltará aquí
        const ushort haltAddr = 0xF000;
        cpu.Memory[haltAddr] = 0x76; // HALT
        const ushort s0 = 0xFF00;
        cpu.Memory[s0] = (byte)(haltAddr & 0xFF);
        cpu.Memory[s0 + 1] = (byte)(haltAddr >> 8);
        cpu.Registers.SP = unchecked((short)s0);

        // "Salta" a main (equivale a CALL main previamente)
        cpu.Registers.PC = entryPc;

        for (int i = 0; i < maxSteps && !cpu.IsHalted; i++)
        {
            cpu.ExecuteNextInstruction();
        }

        if (!cpu.IsHalted)
            throw new TimeoutException("Z80 execution didn't reach HALT within step bound.");

        return cpu;
    }
}

