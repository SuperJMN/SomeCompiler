using CodeGeneration.Model.Classes;
using CSharpFunctionalExtensions;
using SomeCompiler.Generation.Intermediate;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Z80.Core;

namespace SomeCompiler.Z80;

public class Z80Generator
{
    public Result<GeneratedProgram> Generate(IntermediateCodeProgram program)
    {
        var getNames = GetNames(program);
        var addresses = GetMemAddresses(program);
        var table = getNames.Join(addresses, t => t.Item1, y => y.Key, (a, b) => new{ a.Item1, a.Item2, b.Value }).ToDictionary(x => x.Item1, x => new MetaData(x.Item2, x.Value));
        var generator = new Z80AssemblyGenerator(new Z80IntermediateToOpCodeEmitter(new Z80OpCodeEmitter(table)));

        var asm = program
            .Select(c => generator.Generate(c).JoinWithLines())
            .JoinWithLines();
        
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