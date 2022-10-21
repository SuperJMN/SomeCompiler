using System.Collections.Immutable;
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
        var state = await Run("void main() { return 123; }");
        state.StackContents.Should().BeEquivalentTo(new DataMemoryEntry(123));
    }

    [Fact]
    public async Task Assignment()
    {
        var state = await Run("void main() { a = 123; }");
        state.Memory["a"].Should().BeEquivalentTo(new DataMemoryEntry(123));
    }

    [Fact]
    public async Task Multiple_assignment()
    {
        var state = await Run("void main() { a = 1; b = 2; }");
        state.Memory["a"].Should().BeEquivalentTo(new DataMemoryEntry(1));
        state.Memory["b"].Should().BeEquivalentTo(new DataMemoryEntry(2));
    }

    private static IObservable<VirtualMachineState> Run(string code)
    {
        var gen = new CompilerFrontend();
        var generated = gen.Emit(code);
        var vm = new SomeVirtualMachine();
        vm.Load(generated.Value);
        return vm.RunUntilHalted()
            .Timeout(TimeSpan.FromSeconds(0.5))
            .Select(_ => new VirtualMachineState(vm.ExecutionPointer, vm.StackContents, vm.Memory.ToImmutableList(), vm.References));
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
        await sut.RunUntilHalted();
        sut.IsHalted.Should().BeTrue();
    }

    private static async Task RunAndCheck(IntermediateCodeProgram intermediateCodeProgram, string variable, int expected)
    {
        var sut = new SomeVirtualMachine();
        sut.Load(intermediateCodeProgram);

        await sut.RunUntilHalted();

        var mem = sut.GetVariable(variable);
        mem
            .Should().BeOfType<DataMemoryEntry>()
            .Which.Value.Should().Be(expected);
    }
}