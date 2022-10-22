using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model;

public static class CodeFormatter
{
    public static IEnumerable<string> ToCodeFormatContent(this IntermediateCodeProgram program)
    {
        var named = program.NamedReferences().Select(x => ((Reference) x, x.Value));
        var unnamed = program.UnnamedReferences().Select((x, i) => (x, $"T{i+1}"));
        var map = named.Concat(unnamed).ToDictionary(x => x.Item1, tuple => tuple.Item2);
        return program.Select(code => code.ToString(map));
    }

    private static string GetReferenceName(Reference? reference, IDictionary<Placeholder, string> map)
    {
        if (reference is null)
        {
            return "";
        }

        return reference switch
        {
            LabelReference labelReference => labelReference.Label,
            ConstantReference constantReference => constantReference.Constant.ToString(),
            Placeholder dynamicReference => map.GetOrCreate(dynamicReference, () => "T" + (map.Count + 1)),
            NamedReference namedReference => namedReference.Value,
            _ => throw new ArgumentOutOfRangeException(nameof(reference))
        };
    }
}