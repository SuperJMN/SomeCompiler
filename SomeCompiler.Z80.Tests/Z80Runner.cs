using CSharpFunctionalExtensions;
using Konamiman.Z80dotNet;
using Sixty502DotNet;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Z80.Core;

namespace SomeCompiler.Z80.Tests;

public class Z80Runner
{
    public static Result<Z80State> Run(string input)
    {
        var compile = new CompilerFrontend();

        var result = compile
            .Emit(input)
            .MapError(x => x.JoinWithLines())
            .Bind(Generate)
            .Bind(Assemble)
            .Map(Run);

        return result;
    }

    private static Result<byte[]> Assemble(GeneratedProgram program)
    {
        return new Z80Assembler().Assemble(program.Assembly);
    }

    private static Result<GeneratedProgram> Generate(IntermediateCodeProgram x)
    {
        return new Z80Generator().Generate(x);
    }

    private static Z80State Run(byte[] bytes)
    {
        var processor = new Z80Processor();
        processor.Memory.SetContents(0, bytes);
        processor.Start();
        return new Z80State(processor);
    }
}