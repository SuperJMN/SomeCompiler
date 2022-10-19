using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CodeGeneration.Model.Classes;
using CSharpFunctionalExtensions;
using DynamicData;
using FluentAssertions;
using MoreLinq;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Generation.Intermediate.Model.Codes;
using Xunit;

namespace SomeCompiler.Tests;

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
    public async Task MyTest()
    {
        var gen = new CompilerFrontend();
        var generated = gen.Generate("void main() { return 1; }");
        var sut = new VirtualMachine();
        sut.Load(generated.Value);
        await sut.Run();
    }

    [Fact]
    public async Task Halt()
    {
        var code = new IntermediateCodeProgram(new Code[]
        {
            new Halt(),
        });

        var sut = new VirtualMachine();
        sut.Load(code);
        await sut.Run();
        sut.IsHalted.Should().BeTrue();
    }

    private static async Task RunAndCheck(IntermediateCodeProgram intermediateCodeProgram, string variable, int expected)
    {
        var sut = new VirtualMachine();
        sut.Load(intermediateCodeProgram);

        await sut.Run();

        var mem = sut.GetVariable(variable);
        mem
            .Should().BeOfType<DataMemoryEntry>()
            .Which.Value.Should().Be(expected);
    }
}

public class VirtualMachine
{
    private readonly MemoryEntry[] memory = new MemoryEntry[100];
    private Dictionary<Reference, int> variables = new();

    public void Load(IntermediateCodeProgram program)
    {
        variables = program.SelectMany(x => x.GetReferences()).Select((reference, i) => (reference, i)).ToDictionary(t => t.reference, t => 50 + t.i);
        var contents = ToMemory(program);

        Array.ConstrainedCopy(contents.ToArray(), 0, memory, 0, contents.Count);
        ExecutionPointer = 0;
    }

    public long ExecutionPointer { get; private set; }

    public MemoryEntry GetVariable(string name)
    {
        var namedReference = variables.Keys.OfType<NamedReference>().First(x => x.Value == name);
        return memory[variables[namedReference]];
    }

    public IList<InstructionMemoryEntry> ToMemory(IList<Code> program)
    {
        var prepend = new[]{ Maybe.From((Label)null) };

        var labels = prepend.Concat(program.Select(x => Maybe<Label>.From(x as Label)));
        var instructions = labels.Zip(program, (l, i) => new InstructionMemoryEntry(i, l)).Where(x => x.Code is not Label);
        return instructions.ToList();
    }

    private void Step()
    {
        var current = memory[ExecutionPointer];
        ExecuteInstruction(current);
    }

    private void ExecuteInstruction(MemoryEntry memoryEntry)
    {
        if (memoryEntry is not InstructionMemoryEntry ime)
        {
            throw new InvalidOperationException($"{memoryEntry} is not an instruction");
        }

        ExecuteInstruction(ime.Code);
    }

    private void ExecuteInstruction(Code memoryEntry)
    {
        switch (memoryEntry)
        {
            case Add add:
                break;
            case Assign assign:
                break;
            case AssignConstant assignConstant:
                memory[variables[assignConstant.Target]] = new DataMemoryEntry(assignConstant.Source);
                ExecutionPointer++;
                break;
            case Call call:
                var instructionMemoryEntries = from ins in memory.OfType<InstructionMemoryEntry>()
                    from l in ins.Label.ToList()
                    where l.Name == call.Name
                    select ins;

                stack.Push(memory[ExecutionPointer+1]);
                ExecutionPointer = memory.IndexOf(instructionMemoryEntries.First());
                
                break;
            case Divide divide:
                break;
            case EmptyReturn emptyReturn:
                break;
            case Halt halt:
                this.Halt();
                break;
            case Label label:
                break;
            case Multiply multiply:
                break;
            case Return @return:
                var previousInstruction = stack.Pop();
                stack.Push(memory[variables[@return.Reference]]);
                ExecutionPointer = memory.IndexOf(previousInstruction);
                break;
            case Subtract subtract:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(memoryEntry), memoryEntry.ToString());
        }
    }

    private void Halt()
    {
        halted.OnNext(true);
    }

    private readonly BehaviorSubject<bool> halted = new(false);
    private readonly Stack<MemoryEntry> stack = new();

    public IObservable<bool> Halted => halted.AsObservable();
    public bool IsHalted => halted.Value;

    public IObservable<Unit> Run(IScheduler? scheduler = null)
    {
        scheduler ??= Scheduler.Default;

        return Observable.Start(() =>
        {
            while (!IsHalted)
            {
                Step();
            }
        }, scheduler);
    }
}

internal record DataMemoryEntry(int Value) : MemoryEntry;

public record InstructionMemoryEntry(Code Code, Maybe<Label> Label) : MemoryEntry;

public abstract record MemoryEntry;