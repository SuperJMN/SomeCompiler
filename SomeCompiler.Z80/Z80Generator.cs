using System.Text;
using SomeCompiler.Generation.Intermediate.Model;

namespace SomeCompiler.Z80;

public class Z80Generator
{
    public string Generate(IntermediateCodeProgram program)
    {
        StringBuilder strBuilder=new();
        foreach (var code in program)
        {
            if (code.Operator == Operator.Call)
            {
                strBuilder.AppendLine($"CALL {code.Target}");
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