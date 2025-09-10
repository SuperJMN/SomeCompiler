using System;
using System.Linq;
using SomeCompiler.Parser;                    
using SomeCompiler.SemanticAnalysis;         
using SomeCompiler.Generation.Intermediate;  
using SomeCompiler.Z80;                      
using Sixty502DotNet;                        
using Konamiman.Z80dotNet;                   

public class Program
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
        
        // Look for the main function line specifically
        var lines = asm.Split('\n');
        var mainStartIndex = Array.FindIndex(lines, l => l.Trim() == "main:");
        if (mainStartIndex >= 0)
        {
            Console.WriteLine($"Assembly around main (starting at line {mainStartIndex}):");
            for (int i = Math.Max(0, mainStartIndex); i < Math.Min(lines.Length, mainStartIndex + 20); i++)
            {
                Console.WriteLine($"  {i:D3}: {lines[i]}");
            }
        }

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

        // Find main: label address
        var mainAddr = assembled.Value.DebugInfo
            .Where(d => (d.LineText?.Trim() ?? string.Empty) == "main:")
            .Select(d => d.ProgramCounter)
            .FirstOrDefault();
        
        Console.WriteLine($"main: address is at PC: {mainAddr:X4}");
        
        // Show memory dump around main
        Console.WriteLine($"Memory at main:");
        for (int i = 0; i < 50; i++)
        {
            var addr = (ushort)(mainAddr + i);
            var b = bin[addr];
            Console.WriteLine($"  {addr:X4}: {b:X2}");
        }
        
        // Look specifically at the LD HL, 3 instruction at 0x23
        Console.WriteLine($"\nLD HL instruction at 0x23:");
        Console.WriteLine($"  0023: {bin[0x23]:X2} (should be 21 for LD HL, nn)");
        Console.WriteLine($"  0024: {bin[0x24]:X2} (low byte of immediate value)");
        Console.WriteLine($"  0025: {bin[0x25]:X2} (high byte of immediate value)");
        var immediateValue = bin[0x24] | (bin[0x25] << 8);
        Console.WriteLine($"  Immediate value: {immediateValue} (should be 3)");
        
        // Execute with detailed debugging
        int maxSteps = 100;
        for (int i = 0; i < maxSteps && !cpu.IsHalted; i++)
        {
            var prevPC = cpu.Registers.PC;
            var prevHL = (cpu.Registers.H << 8) | cpu.Registers.L;
            var prevSP = cpu.Registers.SP;
            
            // Show every step
            var opcode = cpu.Memory[prevPC];
            Console.WriteLine($"Step {i:D3}: PC={prevPC:X4} OP={opcode:X2} HL={prevHL:X4} SP={prevSP:X4}");

            cpu.ExecuteNextInstruction();

            var currentHL = (cpu.Registers.H << 8) | cpu.Registers.L;
            var currentSP = cpu.Registers.SP;
            
            Console.WriteLine($"        -> PC={cpu.Registers.PC:X4} HL={currentHL:X4} SP={currentSP:X4}");
            
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
