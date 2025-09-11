using System;
using System.Linq;
using SomeCompiler.Parser;                    
using SomeCompiler.SemanticAnalysis;         
using SomeCompiler.Generation.Intermediate;  
using SomeCompiler.Z80;                      
using Sixty502DotNet;                        
using Konamiman.Z80dotNet;                   

public class DebugSimpleReturn
{
    public static void Main()
    {
        var source = @"int main() { return 7; }";

        Console.WriteLine("Source:");
        Console.WriteLine(source);
        Console.WriteLine("\n");

        // Parse
        var parseResult = new SomeParser().Parse(source);
        if (parseResult.IsFailure)
        {
            Console.WriteLine($"Parse error: {parseResult.Error}");
            return;
        }

        // Semantic Analysis
        var analyzed = new SemanticAnalyzer().Analyze(parseResult.Value);
        var root = (SemanticNode)analyzed.Node;
        var allErrors = root.AllErrors.ToList();
        if (allErrors.Count > 0)
        {
            Console.WriteLine($"Semantic errors: {string.Join("\n", allErrors)}");
            return;
        }
        var programNode = (ProgramNode)analyzed.Node;

        // Generate IR
        var ir = new V2IntermediateCodeGenerator().Generate(programNode);
        Console.WriteLine("Intermediate Code:");
        Console.WriteLine(ir);
        Console.WriteLine("\n");

        // Generate ASM
        var asmResult = new Z80Generator().Generate(ir);
        if (asmResult.IsFailure)
        {
            Console.WriteLine($"ASM generation error: {asmResult.Error}");
            return;
        }
        var asm = asmResult.Value.Assembly;
        Console.WriteLine("Assembly:");
        Console.WriteLine(asm);
        Console.WriteLine("\n");

        // Assemble
        var assembler = new Z80Assembler();
        var assembled = assembler.Assemble(asm);
        if (assembled.IsFailure)
        {
            Console.WriteLine($"Assembly error: {assembled.Error}");
            return;
        }

        var bin = assembled.Value.ProgramBinary;
        var entryPc = (ushort)assembled.Value.DebugInfo
            .Where(d => (d.LineText?.Trim() ?? string.Empty) == "main:")
            .Select(d => d.ProgramCounter)
            .DefaultIfEmpty(0)
            .First();

        Console.WriteLine($"Entry PC: {entryPc:X4}");
        
        // Execute step by step
        var cpu = new Z80Processor();
        cpu.Reset();
        cpu.Memory.SetContents(0, bin);

        // Setup halt return address
        const ushort haltAddr = 0xF000;
        cpu.Memory[haltAddr] = 0x76; // HALT
        const ushort s0 = 0xFF00;
        cpu.Memory[s0] = (byte)(haltAddr & 0xFF);
        cpu.Memory[s0 + 1] = (byte)(haltAddr >> 8);
        cpu.Registers.SP = unchecked((short)s0);

        // Jump to main
        cpu.Registers.PC = entryPc;

        Console.WriteLine($"Initial state - PC: {cpu.Registers.PC:X4}, SP: {cpu.Registers.SP:X4}, HL: {cpu.Registers.H:X2}{cpu.Registers.L:X2}, IX: {cpu.Registers.IX:X4}");

        // Execute with detailed debugging
        int maxSteps = 50;
        for (int i = 0; i < maxSteps && !cpu.IsHalted; i++)
        {
            var prevPC = cpu.Registers.PC;
            var prevHL = (cpu.Registers.H << 8) | cpu.Registers.L;
            var prevSP = cpu.Registers.SP;
            var prevIX = cpu.Registers.IX;
            
            // Show memory around IX when it gets set
            if (prevIX != 0)
            {
                var mem_ix_minus1 = cpu.Memory[prevIX - 1];
                var mem_ix_minus2 = cpu.Memory[prevIX - 2];
                Console.WriteLine($"Step {i:D2}: PC={prevPC:X4} HL={prevHL:X4} SP={prevSP:X4} IX={prevIX:X4} [IX-1]={mem_ix_minus1:X2} [IX-2]={mem_ix_minus2:X2}");
            }
            else
            {
                Console.WriteLine($"Step {i:D2}: PC={prevPC:X4} HL={prevHL:X4} SP={prevSP:X4} IX={prevIX:X4}");
            }

            cpu.ExecuteNextInstruction();

            if (cpu.IsHalted)
            {
                break;
            }
        }

        if (!cpu.IsHalted)
        {
            Console.WriteLine("Execution did not halt within step limit");
        }
        else
        {
            var result = (cpu.Registers.H << 8) | cpu.Registers.L;
            Console.WriteLine($"\nFinal result: {result}");
            Console.WriteLine($"Expected: 7");
        }
    }
}
