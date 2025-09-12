using RetroSharp.Z80.Tests.Support;
using Sixty502DotNet;
using Konamiman.Z80dotNet;
using Sixty502DotNet.Shared;

// Let's debug the exact memory addresses
var source = "int main() { return 7; }";
Console.WriteLine($"Source: {source}");

var (bin, entryPc) = Z80E2E.BuildBinaryAndEntryPc(source);
Console.WriteLine($"Entry PC: {entryPc:X4}");

// Let's debug the BuildBinaryAndEntryPc method by recreating it here
Console.WriteLine("\n=== DEBUG ENTRY PC DETECTION ===");
var parseResult = new RetroSharp.Parser.SomeParser().Parse(source);
var analyzed = new RetroSharp.SemanticAnalysis.SemanticAnalyzer().Analyze(parseResult.Value);
var programNode = (RetroSharp.SemanticAnalysis.ProgramNode)analyzed.Node;
var ir = new RetroSharp.Generation.Intermediate.V2IntermediateCodeGenerator().Generate(programNode);
var asmResult = new RetroSharp.Z80.Z80Generator().Generate(ir);
var asm = asmResult.Value.Assembly;

var assembler = new Z80Assembler();
var assembled = assembler.Assemble(asm);
var debugInfo = assembled.Value.DebugInfo;

Console.WriteLine($"Debug info entries: {debugInfo.Count()}");
foreach (var info in debugInfo.Take(20))
{
    Console.WriteLine($"  PC: {info.ProgramCounter:X4}, Line: '{info.LineText?.Trim() ?? "<null>"}'\n    Type: {info.GetType().Name}");
}

// Find main manually
var mainEntries = debugInfo.Where(d => (d.LineText?.Trim() ?? string.Empty) == "main:").ToList();
Console.WriteLine($"\nFound {mainEntries.Count} 'main:' entries:");
foreach (var entry in mainEntries)
{
    Console.WriteLine($"  PC: {entry.ProgramCounter:X4}, Line: '{entry.LineText}'\n    Type: {entry.GetType().Name}");
}

if (mainEntries.Count == 0)
{
    Console.WriteLine("No main: found in debug info - should use PC 0000");
    entryPc = 0;
}
else
{
    entryPc = (ushort)mainEntries.First().ProgramCounter;
    Console.WriteLine($"Using main: entry at PC {entryPc:X4}");
}

// Execute step by step like DebugFactorial to see what happens to memory
var cpu = new Z80Processor();
cpu.Reset();
cpu.Memory.SetContents(0, bin);

// Setup halt return address (same as Z80E2E)
const ushort haltAddr = 0xF000;
cpu.Memory[haltAddr] = 0x76; // HALT
const ushort s0 = 0xFF00;
cpu.Memory[s0] = (byte)(haltAddr & 0xFF);
cpu.Memory[s0 + 1] = (byte)(haltAddr >> 8);
cpu.Registers.SP = unchecked((short)s0);
cpu.Registers.PC = entryPc;

Console.WriteLine($"\nInitial state:");
Console.WriteLine($"PC: {cpu.Registers.PC:X4}, SP: {cpu.Registers.SP:X4}, IX: {cpu.Registers.IX:X4}, HL: {(cpu.Registers.H << 8) | cpu.Registers.L:X4}");

// Execute until we set up IX
for (int i = 0; i < 20; i++)
{
    var prevPC = cpu.Registers.PC;
    var prevSP = cpu.Registers.SP;
    var prevIX = cpu.Registers.IX;
    var prevHL = (cpu.Registers.H << 8) | cpu.Registers.L;
    
    if (cpu.IsHalted) break;
    
    cpu.ExecuteNextInstruction();
    
    var currentSP = cpu.Registers.SP;
    var currentIX = cpu.Registers.IX;
    var currentHL = (cpu.Registers.H << 8) | cpu.Registers.L;
    
    Console.WriteLine($"Step {i:D2}: PC {prevPC:X4} -> {cpu.Registers.PC:X4}, SP {prevSP:X4} -> {currentSP:X4}, IX {prevIX:X4} -> {currentIX:X4}, HL {prevHL:X4} -> {currentHL:X4}");
    
    // When IX is set up, show memory around it
    if (currentIX != 0 && prevIX == 0)
    {
        Console.WriteLine($"  IX now points to {currentIX:X4}");
        Console.WriteLine($"  Memory at IX-2 ({(currentIX-2):X4}): {cpu.Memory[currentIX-2]:X2}");
        Console.WriteLine($"  Memory at IX-1 ({(currentIX-1):X4}): {cpu.Memory[currentIX-1]:X2}");
        Console.WriteLine($"  Memory at IX+0 ({currentIX:X4}): {cpu.Memory[currentIX]:X2}");
        Console.WriteLine($"  Memory at IX+1 ({(currentIX+1):X4}): {cpu.Memory[currentIX+1]:X2}");
    }
    
    // Show memory content when we write to IX-1, IX-2
    if (currentIX != 0)
    {
        var memIXminus2 = cpu.Memory[currentIX-2];
        var memIXminus1 = cpu.Memory[currentIX-1];
        if (memIXminus1 != 0 || memIXminus2 != 0)
        {
            Console.WriteLine($"  [IX-2]={memIXminus2:X2} [IX-1]={memIXminus1:X2}");
        }
    }
}

var finalResult = (cpu.Registers.H << 8) | cpu.Registers.L;
Console.WriteLine($"\nFinal result: {finalResult} (expected 7)");
