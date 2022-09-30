using System.Text;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Z80;

public class Z80Generator
{
    public string Generate(IntermediateCodeProgram program)
    {
        var map = program.SelectMany(x => x.GetReferences()).Select((reference, i) => (reference, i)).ToDictionary(t => t.reference, t => t.i * 2 + 0x30);

        StringBuilder strBuilder=new();
        foreach (var code in program)
        {
            switch (code)
            {
                case Add add:
                    break;
                case Assign assign:
                    break;
                case AssignConstant assignConstant:
                    strBuilder.AppendLine($"LD hl, {map[assignConstant.Target]:X}h");
                    strBuilder.AppendLine($"LD (hl), {assignConstant.Source}");
                    break;
                case Call call:
                    strBuilder.AppendLine($"CALL {call.Name}");
                    break;
                case Divide divide:
                    break;
                case EmptyReturn emptyReturn:
                    strBuilder.AppendLine("RET");
                    break;
                case Halt halt:
                    strBuilder.AppendLine("HALT");
                    break;
                case Label label:
                    strBuilder.AppendLine($"{label.Name}:");
                    break;
                case Multiply multiply:
                    break;
                case Return r:
                    break;
                case Subtract subtract:
                    break;
            }

            //if (code.Operator == Operator.Call)
            //{
            //    strBuilder.AppendLine($"CALL {((LabelReference)code.Target).Label}");
            //}

            //if (code.Operator == Operator.Halt)
            //{
            //    strBuilder.AppendLine("HALT");
            //}

            //if (code.Operator == Operator.Label)
            //{
            //    strBuilder.AppendLine($"LABEL {code.Target}");
            //}

            //if (code.Operator == Operator.Equal)
            //{
            //    strBuilder.AppendLine($"LD {code.Target}, {code.Left}");
            //}

            //if (code.Operator == Operator.Add)
            //{
            //    strBuilder.AppendLine($"ADD {code.Target}, {code.Left}");
            //}

            //if (code.Operator == Operator.Return)
            //{
            //    strBuilder.AppendLine("RET");
            //}
        }

        return strBuilder.ToString();
    }
}