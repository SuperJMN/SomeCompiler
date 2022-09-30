using CodeGeneration.Model.Classes;
using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Generation.Intermediate.Model;

public static class CodeFormatter
{
    public static IEnumerable<string> ToCodeFormatContent(ICollection<Code> codes)
    {
        var map = codes.SelectMany(x => x.GetReferences()).Select((x, i) => (x, i)).ToDictionary(tuple => tuple.x, x => "T" + (x.i + 1));
        return codes.Select(code => code.ToString(map));
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