using System;
using System.Linq;
using SomeCompiler.Parser;                    
using SomeCompiler.SemanticAnalysis;         
using SomeCompiler.Generation.Intermediate;  
using SomeCompiler.Z80;                      
using Sixty502DotNet;                        
using Konamiman.Z80dotNet;                   

public class DebugFactorial
{
    public static void Main()
    {
        var source = @"int fact(int n) { 
            if (n == 0) { 
                return 1; 
            } else { 
                return n * fact(n - 1); 
            } 
        } 
        int main() { 
            return fact(3); 
        }";

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

        Console.WriteLine($"Initial state - PC: {cpu.Registers.PC:X4}, SP: {cpu.Registers.SP:X4}, HL: {cpu.Registers.H:X2}{cpu.Registers.L:X2}");

        // Execute with debugging
        int maxSteps = 1000;
        for (int i = 0; i < maxSteps && !cpu.IsHalted; i++)
        {
            var prevPC = cpu.Registers.PC;
            var prevHL = (cpu.Registers.H << 8) | cpu.Registers.L;
            var prevSP = cpu.Registers.SP;

            cpu.ExecuteNextInstruction();

            var currentHL = (cpu.Registers.H << 8) | cpu.Registers.L;
            var currentSP = cpu.Registers.SP;

            // Show important changes
            if (prevHL != currentHL || Math.Abs(currentSP - prevSP) > 10 || i < 50)
            {
                Console.WriteLine($"Step {i:D3}: PC {prevPC:X4} -> {cpu.Registers.PC:X4}, HL: {prevHL:X4} -> {currentHL:X4}, SP: {prevSP:X4} -> {currentSP:X4}");
            }

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
            Console.WriteLine($"Expected: 6 for fact(3)");
        }
    }
}
