using CodeGeneration.Model.Classes;

namespace SomeCompiler.Intermediate.Model;

public static class CodeFormatter
{
    public static IEnumerable<string> ToCodeFormatContent(IEnumerable<Code> codes)
    {
        var map = new Dictionary<Placeholder, string>();

        return codes.Select(code => Format(code, map));
    }

    private static string Format(Code code, IDictionary<Placeholder, string> map)
    {
        if (code.Right is null)
        {
            return $"{GetReferenceName(code.Destination, map)} {code.Operator} {GetReferenceName(code.Left, map)} {GetReferenceName(code.Right, map)}";
        }

        return $"{GetReferenceName(code.Destination, map)} = {GetReferenceName(code.Left, map)} {code.Operator} {GetReferenceName(code.Right, map)}";
    }

    private static string GetReferenceName(Reference? reference, IDictionary<Placeholder, string> map)
    {
        if (reference is null)
        {
            return "";
        }

        return reference switch
        {
            Placeholder dynamicReference => map.GetOrCreate(dynamicReference, () => "T" + map.Count),
            NamedReference namedReference => namedReference.Value,
            _ => throw new ArgumentOutOfRangeException(nameof(reference))
        };
    }
}