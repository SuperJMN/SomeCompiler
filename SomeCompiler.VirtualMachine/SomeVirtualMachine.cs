using System.Collections.Immutable;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CodeGeneration.Model.Classes;
using SomeCompiler.Generation.Intermediate;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Generation.Intermediate.Model.Codes;
using SomeCompiler.VirtualMachine.Commands;

namespace SomeCompiler.VirtualMachine;

public class SomeVirtualMachine : IMachine
{
    private readonly BehaviorSubject<bool> halted = new(false);
    private readonly MemoryEntry[] memory = new MemoryEntry[100];
    private readonly Stack<MemoryEntry> stack = new();
    private Dictionary<Reference, int> variables = new();

    public Dictionary<string, int> References =>
        (from r in Variables let rf = r.Key as NamedReference where rf != null select new { Name = rf.Value, Index = r.Value })
        .ToDictionary(x => x.Name, x => x.Index);

    public IReadOnlyList<MemoryEntry> StackContents => Stack.ToImmutableList();
    public IList<MemoryEntry> Memory => memory;

    public int ExecutionPointer { get; set; }

    public bool IsHalted => halted.Value;

    public Stack<MemoryEntry> Stack => stack;

    public Dictionary<Reference, int> Variables
    {
        get => variables;
        private set => variables = value;
    }

    public void Load(IntermediateCodeProgram program)
    {
        Variables = program.IndexedReferences().ToList().ToDictionary(t => t.Reference, t => 50 + t.Index);

        var contents = ToMemory(program);

        Array.ConstrainedCopy(contents.ToArray(), 0, memory, 0, contents.Count);
        ExecutionPointer = 0;
    }

    public MemoryEntry GetVariable(string name)
    {
        var namedReference = Variables.Keys.OfType<NamedReference>().First(x => x.Value == name);
        return memory[Variables[namedReference]];
    }

    private static IList<InstructionMemoryEntry> ToMemory(IList<Code> program)
    {
        return program
            .AsLabeled()
            .Select(lc => new InstructionMemoryEntry(lc.Code, lc.Label))
            .ToList();
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

        var command = ToCommand(ime);
        command.Execute();
    }

    private Command ToCommand(InstructionMemoryEntry instructionMemoryEntry)
    {
        return instructionMemoryEntry.Code switch
        {
            Add add => new AddCommand(this, add.Left, add.Right, add.Target),
            Assign assign => new AssignReferenceCommand(this, assign.Source, assign.Target),
            AssignConstant assignConstant => new AssignConstantCommand(this, assignConstant.Source, assignConstant.Target),
            Call call => new CallCommand(this, call.Name),
            Divide divide => new DivideCommand(this, divide.Left, divide.Right, divide.Target),
            EmptyReturn emptyReturn => new ReturnCommand(this),
            Halt halt => new HaltCommand(this),
            Multiply multiply => new MultiplyCommand(this, multiply.Left, multiply.Right, multiply.Target),
            Return @return => new ReturnReferenceCommand(this, @return.Reference),
            Subtract subtract => new SubtractCommand(this, subtract.Left, subtract.Right, subtract.Target),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void Halt()
    {
        halted.OnNext(true);
    }
}