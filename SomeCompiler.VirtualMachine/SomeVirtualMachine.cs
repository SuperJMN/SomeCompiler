using System.Collections.Immutable;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CodeGeneration.Model.Classes;
using CSharpFunctionalExtensions;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.VirtualMachine;

public class SomeVirtualMachine
{
    private readonly BehaviorSubject<bool> halted = new(false);
    private readonly MemoryEntry[] memory = new MemoryEntry[100];
    private readonly Stack<MemoryEntry> stack = new();
    private Dictionary<Reference, int> variables = new();

    public Dictionary<string, int> References => 
        (from r in variables let rf = r.Key as NamedReference where rf != null select new { Name = rf.Value, Index = r.Value })
        .ToDictionary(x => x.Name, x => x.Index);

    public IReadOnlyList<MemoryEntry> StackContents => stack.ToImmutableList();
    public IReadOnlyList<MemoryEntry> Memory => memory.ToImmutableList();

    public long ExecutionPointer { get; private set; }

    public bool IsHalted => halted.Value;

    public void Load(IntermediateCodeProgram program)
    {
        var valueTuples = program
            .SelectMany(x => x.GetReferences())
            .ToImmutableHashSet()
            .Select((r, i) => (Reference: r, i)).ToList();

        variables = valueTuples.ToDictionary(t => t.Reference, t => 50 + t.i);

        var contents = ToMemory(program);

        Array.ConstrainedCopy(contents.ToArray(), 0, memory, 0, contents.Count);
        ExecutionPointer = 0;
    }

    public MemoryEntry GetVariable(string name)
    {
        var namedReference = variables.Keys.OfType<NamedReference>().First(x => x.Value == name);
        return memory[variables[namedReference]];
    }

    public IList<InstructionMemoryEntry> ToMemory(IList<Code> program)
    {
        var prepend = new[] { Maybe.From((Label) null) };

        var labels = prepend.Concat(program.Select(x => Maybe<Label>.From(x as Label)));
        var instructions = labels.Zip(program, (l, i) => new InstructionMemoryEntry(i, l)).Where(x => x.Code is not Label);
        return instructions.ToList();
    }

    public IObservable<Unit> RunUntilHalted(IScheduler? scheduler = null)
    {
        scheduler ??= Scheduler.Default;

        return Observable.Start(RunLoop, scheduler);
    }

    private void RunLoop()
    {
        while (!IsHalted)
        {
            Step();
        }
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

                stack.Push(memory[ExecutionPointer + 1]);
                ExecutionPointer = memory.ToList().IndexOf(instructionMemoryEntries.First());

                break;
            case Divide divide:
                break;
            case EmptyReturn emptyReturn:
                break;
            case Halt halt:
                Halt();
                break;
            case Label label:
                break;
            case Multiply multiply:
                break;
            case Return ret:
                var previousInstruction = stack.Pop();
                stack.Push(memory[variables[ret.Reference]]);
                ExecutionPointer = memory.ToList().IndexOf(previousInstruction);
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
}