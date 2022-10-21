using System.Collections.Immutable;
using CodeGeneration.Model.Classes;
using SomeCompiler.Generation.Intermediate.Model;

namespace SomeCompiler.Generation.Intermediate;

public static class IntermediateProgramExtensions
{
    public static IEnumerable<(int Index, Reference Reference)> IndexedReferences(this IntermediateCodeProgram program)
    {
        return program
            .SelectMany(x => x.GetReferences())
            .ToImmutableHashSet()
            .Select((r, i) => (Index: i, Reference: r));
    }
}