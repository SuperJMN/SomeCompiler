using CSharpFunctionalExtensions;
using Konamiman.Z80dotNet;
using Sixty502DotNet;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Z80.Core;
using Xunit.Abstractions;
using Zafiro.Core.Mixins;

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
        var compile = new Compiler();

        var result = compile
            .Emit(input)
            .MapError(x => x.JoinWithLines())
            .Bind(Generate)
            .Bind(Assemble)
            .Map(Run);

        return result;
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
        var processor = new Z80Processor();
        processor.Memory.SetContents(0, assemblyData.ProgramBinary);
        processor.Start();
        return new Z80State(processor);
    }
}