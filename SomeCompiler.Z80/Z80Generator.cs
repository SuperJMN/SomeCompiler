using System.Text;
using SomeCompiler.Generation.Intermediate.Model;

namespace SomeCompiler.Z80;

public class Z80Generator
{
    public string Generate(IntermediateCodeProgram program)
    {
        var refToMemAddressMap = program.Select((code, i) => (code, i)).ToDictionary(t => t.code, t => t.i * 2 + 0x30);

        StringBuilder strBuilder=new();
        foreach (var code in program)
        {
            if (code.Operator == Operator.Call)
            {
                strBuilder.AppendLine($"CALL {((LabelReference)code.Target).Label}");
            }

            if (code.Operator == Operator.Halt)
            {
                strBuilder.AppendLine("HALT");
            }

            if (code.Operator == Operator.Label)
            {
                strBuilder.AppendLine($"LABEL {code.Target}");
            }

            if (code.Operator == Operator.Equal)
            {
                strBuilder.AppendLine($"LD {code.Target}, {code.Left}");
            }

            if (code.Operator == Operator.Add)
            {
                strBuilder.AppendLine($"ADD {code.Target}, {code.Left}");
            }

            if (code.Operator == Operator.Return)
            {
                strBuilder.AppendLine("RET");
            }
        }

        return strBuilder.ToString();
    }
}