using CodeGeneration.Model.Classes;
using CSharpFunctionalExtensions;
using SomeCompiler.Generation.Intermediate;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Generation.Intermediate.Model.Codes;
using SomeCompiler.Z80.Core;

namespace SomeCompiler.Z80;

public class Z80Generator
{
    public IList<LabeledInstruction> AsLabeled(IList<Code> program)
    {
        var prepend = new[] { Maybe.From((Label) null) };

        var labels = prepend.Concat(program.Select(x => Maybe<Label>.From(x as Label)));
        var instructions = labels.Zip(program, (l, i) => new LabeledInstruction(l, i)).Where(x => x.Code is not Label);
        return instructions.ToList();
    }

    public Result<GeneratedProgram> Generate(IntermediateCodeProgram program)
    {
        var getNames = GetNames(program);
        var addresses = GetMemAddresses(program);
        var table = getNames.Join(addresses, t => t.Item1, y => y.Key, (a, b) => new{ a.Item1, a.Item2, b.Value }).ToDictionary(x => x.Item1, x => new MetaData(x.Item2, x.Value));
        var codes = AsLabeled(program);

        var generator = new Z80LabeledAssemblyGenerator(new Z80IntermediateToOpCodeEmitter(new Z80OpCodeEmitter(table)));

        var asm = string.Join(Environment.NewLine, codes.Select(c => generator.Generate(c)));
        return new GeneratedProgram(asm);
    }

    private static Dictionary<Reference, int> GetMemAddresses(IntermediateCodeProgram program)
    {
        return program.IndexedReferences().ToDictionary(t => t.Reference, t => t.Index * 2 + 0x30);
    }

    private IEnumerable<(Reference, string)> GetNames(IntermediateCodeProgram program)
    {
        var named = program.NamedReferences().Select(x => ((Reference) x, x.Value));
        var unnamed = program.UnnamedReferences().Select((x, i) => (x, $"T{i+1}"));
        return named.Concat(unnamed);
    }
}