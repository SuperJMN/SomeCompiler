using System.Reactive.Linq;
using FluentAssertions;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.VirtualMachine.Tests;

public class VirtualMachineTests
{
    [Fact]
    public Task Test()
    {
        var code = new IntermediateCodeProgram(new Code[]
        {
            new AssignConstant(new NamedReference("A"), 1),
            new AssignConstant(new NamedReference("B"), 2),
            new Halt(),
        });

        return RunAndCheck(code, "B", 2);
    }

    [Fact]
    public async Task Return_constant_puts_data_in_stack()
    {
        var status = await Run("void main() { return 123; }");
        status.StackContents.Should().BeEquivalentTo(new DataMemoryEntry(123));
    }

    private static async Task<VirtualMachineState> Run(string code)
    {
        var gen = new CompilerFrontend();
        var generated = gen.Emit(code);
        var vm = new SomeVirtualMachine();
        vm.Load(generated.Value);
        await vm.Run();
        
        return new VirtualMachineState(vm.ExecutionPointer, vm.StackContents, vm.Memory);
    }

    [Fact]
    public async Task Halt()
    {
        var code = new IntermediateCodeProgram(new Code[]
        {
            new Halt(),
        });

        var sut = new SomeVirtualMachine();
        sut.Load(code);
        await sut.Run();
        sut.IsHalted.Should().BeTrue();
    }

    private static async Task RunAndCheck(IntermediateCodeProgram intermediateCodeProgram, string variable, int expected)
    {
        var sut = new SomeVirtualMachine();
        sut.Load(intermediateCodeProgram);

        await sut.Run();

        var mem = sut.GetVariable(variable);
        mem
            .Should().BeOfType<DataMemoryEntry>()
            .Which.Value.Should().Be(expected);
    }
}