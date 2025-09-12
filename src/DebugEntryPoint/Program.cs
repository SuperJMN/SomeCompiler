using System;
using System.Linq;
using RetroSharp.Parser;                    
using RetroSharp.SemanticAnalysis;         
using RetroSharp.Generation.Intermediate;  
using RetroSharp.Z80;                      
using Sixty502DotNet;                        
using Konamiman.Z80dotNet;
using Sixty502DotNet.Shared;

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

// Parsear y compilar
var parseResult = new SomeParser().Parse(source);
if (parseResult.IsFailure)
    throw new InvalidOperationException(parseResult.Error);
var programSyntax = parseResult.Value;

var analyzed = new SemanticAnalyzer().Analyze(programSyntax);
var root = (SemanticNode)analyzed.Node;
var allErrors = root.AllErrors.ToList();
if (allErrors.Count > 0)
    throw new InvalidOperationException(string.Join("\n", allErrors));
var programNode = (ProgramNode)analyzed.Node;

var ir = new V2IntermediateCodeGenerator().Generate(programNode);
var asmResult = new Z80Generator().Generate(ir);
if (asmResult.IsFailure)
    throw new InvalidOperationException(asmResult.Error);
var asm = asmResult.Value.Assembly;

Console.WriteLine("==== ASSEMBLY ====");
Console.WriteLine(asm);
Console.WriteLine();

// Ensamblar con debug
var assembler = new Z80Assembler();
var assembled = assembler.Assemble(asm);
if (assembled.IsFailure)
    throw new InvalidOperationException(assembled.Error);

var bin = assembled.Value.ProgramBinary;
Console.WriteLine($"==== BINARY LENGTH: {bin.Length} ====");

// Revisar debug info
Console.WriteLine("==== DEBUG INFO ====");
foreach (var debug in assembled.Value.DebugInfo)
{
    Console.WriteLine($"PC: {debug.ProgramCounter:X4}, Line: '{debug.LineText?.Trim()}'");
}
Console.WriteLine();

// Buscar entry point usando la lógica corregida
var entryPc = (ushort)assembled.Value.DebugInfo
    .Where(d => (d.LineText?.Trim() ?? string.Empty) == "main:")
    .Select(d => d.ProgramCounter)
    .DefaultIfEmpty(0)
    .First();

Console.WriteLine($"==== ENTRY POINT FROM DEBUG: {entryPc:X4} ====");

// If not found in debug info, look for main: in assembly and find correct PC
if (entryPc == 0)
{
    var lines = asm.Split('\n');
    for (int i = 0; i < lines.Length; i++)
    {
        if (lines[i].Trim() == "main:")
        {
            Console.WriteLine($"Found main: at line {i}: '{lines[i]}'");
            // Found main: label, now find the first instruction after it in debug info
            // Look for the first actual instruction (not a label) after the main: line
            for (int j = i + 1; j < lines.Length; j++)
            {
                var nextLine = lines[j].Trim();
                if (!string.IsNullOrEmpty(nextLine) && !nextLine.EndsWith(':'))
                {
                    Console.WriteLine($"First instruction after main: '{nextLine}'");
                    // This should be the first instruction of main
                    // Find it in debug info to get the correct PC, but search only after 
                    // the previous function to avoid matching the same instruction in other functions
                    var instruction = nextLine.Split('\t')[0];
                    
                    // Find all debug entries that match this instruction
                    var matchingEntries = assembled.Value.DebugInfo
                        .Where(d => d.LineText?.Trim().StartsWith(instruction) == true)
                        .OrderBy(d => d.ProgramCounter)
                        .ToList();
                    
                    Console.WriteLine($"Found {matchingEntries.Count} matching entries for '{instruction}'");
                    foreach (var entry in matchingEntries)
                    {
                        Console.WriteLine($"  PC: {entry.ProgramCounter:X4} - {entry.LineText?.Trim()}");
                    }
                    
                    // Take the last matching entry (should be main's version)
                    // Or find the one with the highest PC address
                    if (matchingEntries.Any())
                    {
                        entryPc = (ushort)matchingEntries.Last().ProgramCounter;
                        Console.WriteLine($"Using last matching entry at PC {entryPc:X4}");
                        break;
                    }
                }
            }
            break;
        }
    }
}

Console.WriteLine($"==== FINAL ENTRY POINT: {entryPc:X4} ====");

// Ejecutar con debug
var cpu = new Z80Processor();
cpu.Reset();
cpu.Memory.SetContents(0, bin);

// Preparar retorno a HALT
const ushort haltAddr = 0xF000;
cpu.Memory[haltAddr] = 0x76; // HALT
const ushort s0 = 0xFF00;
cpu.Memory[s0] = (byte)(haltAddr & 0xFF);
cpu.Memory[s0 + 1] = (byte)(haltAddr >> 8);
cpu.Registers.SP = unchecked((short)s0);

Console.WriteLine($"==== STARTING EXECUTION AT PC: {entryPc:X4} ====");
Console.WriteLine($"Initial SP: {cpu.Registers.SP:X4}");
Console.WriteLine($"Initial HL: {(cpu.Registers.H << 8) | (cpu.Registers.L & 0xFF)}");

cpu.Registers.PC = entryPc;

for (int i = 0; i < 100000 && !cpu.IsHalted; i++)
{
    if (i < 10 || i % 1000 == 0) // Log primeras 10 instrucciones y luego cada 1000
    {
        Console.WriteLine($"Step {i}: PC={cpu.Registers.PC:X4}, HL={(cpu.Registers.H << 8) | (cpu.Registers.L & 0xFF)}, SP={cpu.Registers.SP:X4}");
    }
    cpu.ExecuteNextInstruction();
}

if (cpu.IsHalted)
{
    var result = (cpu.Registers.H << 8) | (cpu.Registers.L & 0xFF);
    Console.WriteLine($"==== EXECUTION COMPLETED ====");
    Console.WriteLine($"Final HL: {result} (expected: 6)");
    Console.WriteLine($"Final PC: {cpu.Registers.PC:X4}");
    Console.WriteLine($"Final SP: {cpu.Registers.SP:X4}");
}
else
{
    Console.WriteLine("==== EXECUTION TIMED OUT ====");
}
