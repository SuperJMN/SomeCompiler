using CSharpFunctionalExtensions;
using Konamiman.Z80dotNet;
using Sixty502DotNet;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Generation.Intermediate.Model.Codes;
using SomeCompiler.Z80.Core;
using Xunit.Abstractions;
using Zafiro.Core.Mixins;
using IRCode = SomeCompiler.Generation.Intermediate.Model.Codes.Code;
using IRProgram = SomeCompiler.Generation.Intermediate.Model.IntermediateCodeProgram;

namespace SomeCompiler.Z80.Tests;

public class Z80Runner
{
    private readonly ITestOutputHelper logger;

    public Z80Runner(ITestOutputHelper logger)
    {
        this.logger = logger;
    }
    
    public Result<Z80State> Run(string input)
    {
        return BuildProgram(input)
            .Bind(Generate)
            .Bind(Assemble)
            .Map(Run);
    }

    private Result<IntermediateCodeProgram> BuildProgram(string input)
    {
        // Very small ad-hoc builder supporting: int main() { return <int> [op <int>]*; }
        try
        {
            var insideBraces = input[(input.IndexOf('{') + 1)..input.IndexOf('}')];
            var returnPart = insideBraces.Trim().Replace("return", string.Empty).Replace(";", string.Empty).Trim();
            var tokens = returnPart.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var codes = new List<IRCode>();
            codes.Add(new SomeCompiler.Generation.Intermediate.Model.Codes.Label("main"));

            // Build references for operands
            var valueRefs = new List<CodeGeneration.Model.Classes.Reference>();
            for (int i = 0; i < tokens.Length; i += 2)
            {
                if (!int.TryParse(tokens[i], out var value))
                    throw new InvalidOperationException("Only integer literals are supported in tests");
                var temp = new Placeholder();
                codes.Add(new AssignConstant(temp, value));
                valueRefs.Add(temp);
            }

            // Apply operators left to right
            CodeGeneration.Model.Classes.Reference current = valueRefs[0];
            for (int i = 1, opIndex = 1; i < valueRefs.Count; i++, opIndex += 2)
            {
                var op = tokens[opIndex];
                var next = valueRefs[i];
                var target = new Placeholder();
                if (op == "+")
                {
                    codes.Add(new Add(target, current, next));
                }
                else if (op == "*")
                {
                    codes.Add(new Multiply(target, current, next));
                }
                else
                {
                    throw new NotSupportedException($"Operator '{op}' not supported in tests");
                }
                current = target;
            }

            codes.Add(new Return(current));

            var program = new IRProgram();
            program.AddRange(codes);
            return program;
        }
        catch (Exception ex)
        {
            return Result.Failure<IntermediateCodeProgram>(ex.Message);
        }
    }

    private Result<AssemblyData> Assemble(GeneratedProgram program)
    {
        logger.WriteLine(program.Assembly);
        var assemble = new Z80Assembler().Assemble(program.Assembly);
        return assemble;
    }

    private Result<GeneratedProgram> Generate(IntermediateCodeProgram x)
    {
        return new Z80Generator().Generate(x);
    }

    private Z80State Run(AssemblyData assemblyData)
    {
        // Load assembled program at a fixed address
        // Load at origin 0 to match assembler default
        var cpu = new Z80Processor();
        cpu.Reset();
        cpu.Memory.SetContents(0, assemblyData.ProgramBinary);

        // Resolve entry offset (origin-relative) and setup a HALT return address on the stack
        var entryOffset = assemblyData.DebugInfo
            .Where(d => (d.LineText?.Trim() ?? string.Empty).Equals("main:", StringComparison.Ordinal))
            .Select(d => d.ProgramCounter)
            .DefaultIfEmpty(0)
            .First();
        const ushort haltAddr = 0xF000;
        cpu.Memory[haltAddr] = 0x76; // HALT
        const ushort s0 = 0xFF00;
        cpu.Memory[s0] = (byte)(haltAddr & 0xFF);
        cpu.Memory[s0 + 1] = (byte)(haltAddr >> 8);
        cpu.Registers.SP = unchecked((short)s0);
        cpu.Registers.PC = (ushort)entryOffset;

        // Step bounded number of instructions until HALT
        const int MaxSteps = 20000;
        for (int i = 0; i < MaxSteps; i++)
        {
            cpu.ExecuteNextInstruction();
            if (cpu.IsHalted) break;
        }
        return new Z80State(cpu);
    }
}
