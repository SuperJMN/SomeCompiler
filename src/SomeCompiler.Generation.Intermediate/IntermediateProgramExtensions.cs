using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Generation.Intermediate;

public static class IntermediateProgramExtensions
{
    public static IEnumerable<(int Index, CodeGeneration.Model.Classes.Reference Reference)> IndexedReferences(this SomeCompiler.Generation.Intermediate.Model.IntermediateCodeProgram program)
    {
        return program
            .Cast<SomeCompiler.Generation.Intermediate.Model.Codes.Code>()
            .SelectMany(x => x.GetReferences())
            .Distinct()
            .Select((r, i) => (Index: i, Reference: r));
    }

    public static IEnumerable<SomeCompiler.Generation.Intermediate.Model.NamedReference> NamedReferences(this SomeCompiler.Generation.Intermediate.Model.IntermediateCodeProgram program)
    {
        return program
            .Cast<SomeCompiler.Generation.Intermediate.Model.Codes.Code>()
            .SelectMany(x => x.GetReferences())
            .OfType<SomeCompiler.Generation.Intermediate.Model.NamedReference>()
            .Distinct();
    }

    public static IEnumerable<CodeGeneration.Model.Classes.Reference> UnnamedReferences(this SomeCompiler.Generation.Intermediate.Model.IntermediateCodeProgram program)
    {
        return program
            .Cast<SomeCompiler.Generation.Intermediate.Model.Codes.Code>()
            .SelectMany(x => x.GetReferences())
            .OfType<SomeCompiler.Generation.Intermediate.Model.Placeholder>()
            .Distinct();
    }

    public static IEnumerable<string> ToTextFormatContent(this SomeCompiler.Generation.Intermediate.Model.IntermediateCodeProgram program)
    {
        var named = program.NamedReferences().Select(x => ((CodeGeneration.Model.Classes.Reference) x, x.Value));
        var unnamed = program.UnnamedReferences().Select((x, i) => (x, $"T{i+1}"));
        var map = named.Concat(unnamed).ToDictionary(x => x.Item1, tuple => tuple.Item2);
        return program.Cast<SomeCompiler.Generation.Intermediate.Model.Codes.Code>().Select(code => code.ToString(map));
    }
}
