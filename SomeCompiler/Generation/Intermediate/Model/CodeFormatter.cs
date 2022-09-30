using CodeGeneration.Model.Classes;

namespace SomeCompiler.Generation.Intermediate.Model;

public static class CodeFormatter
{
    public static IEnumerable<string> ToCodeFormatContent(IEnumerable<Code> codes)
    {
        var map = new Dictionary<Placeholder, string>();

        return codes.Select(code => Format(code, map));
    }

    private static string Format(Code code, IDictionary<Placeholder, string> map)
    {
        if (code.Right is null && code.Left is null && code.Target is null)
        {
            return $"{code.Operator}";
        }

        if (code.Right is null && code.Left is null)
        {
            return $"{code.Operator} {GetReferenceName(code.Target, map)}";
        }

        if (code.Right is null)
        {
            return $"{GetReferenceName(code.Target, map)} {code.Operator} {GetReferenceName(code.Left, map)} {GetReferenceName(code.Right, map)}";
        }

        return $"{GetReferenceName(code.Target, map)} = {GetReferenceName(code.Left, map)} {code.Operator} {GetReferenceName(code.Right, map)}";
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