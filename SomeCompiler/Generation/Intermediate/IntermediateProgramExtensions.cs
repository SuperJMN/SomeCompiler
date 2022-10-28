using CodeGeneration.Model.Classes;
using CSharpFunctionalExtensions;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Generation.Intermediate;

public static class IntermediateProgramExtensions
{
    public static IEnumerable<(int Index, Reference Reference)> IndexedReferences(this IntermediateCodeProgram program)
    {
        return program
            .SelectMany(x => x.GetReferences())
            .Distinct()
            .Select((r, i) => (Index: i, Reference: r));
    }

    public static IEnumerable<NamedReference> NamedReferences(this IntermediateCodeProgram program)
    {
        return program
            .SelectMany(x => x.GetReferences())
            .OfType<NamedReference>()
            .Distinct();
    }

    public static IEnumerable<Reference> UnnamedReferences(this IntermediateCodeProgram program)
    {
        return program
            .SelectMany(x => x.GetReferences())
            .OfType<Placeholder>()
            .Distinct();
    }

    public static IEnumerable<string> ToTextFormatContent(this IntermediateCodeProgram program)
    {
        var named = program.NamedReferences().Select(x => ((Reference) x, x.Value));
        var unnamed = program.UnnamedReferences().Select((x, i) => (x, $"T{i+1}"));
        var map = named.Concat(unnamed).ToDictionary(x => x.Item1, tuple => tuple.Item2);
        return program.Select(code => code.ToString(map));
    }

    public static IList<LabeledCode> AsLabeled(this IList<Code> program)
    {
        var prepend = new[] { Maybe.From((Label) null) };

        var labels = prepend.Concat(program.Select(x => Maybe<Label>.From(x as Label)));
        var instructions = labels.Zip(program, (l, c) => new LabeledCode(l, c)).Where(x => x.Code is not Label);
        return instructions.ToList();
    }
}