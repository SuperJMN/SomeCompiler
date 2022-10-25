using System.Text;
using CSharpFunctionalExtensions;
using SomeCompiler.Generation.Intermediate;
using SomeCompiler.Generation.Intermediate.Model;
using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.Z80;

public class Z80Generator
{
    public Result<GeneratedProgram> Generate(IntermediateCodeProgram program)
    {
        var map = program.IndexedReferences().ToDictionary(t => t.Reference, t => t.Index * 2 + 0x30);

        StringBuilder strBuilder=new();
        foreach (var code in program)
        {
            switch (code)
            {
                case Add add:

                    // Example
                    // LD      hl,(20h) 
                    // LD      a,l 
                    // LD      hl,(22h) 
                    // LD      b,l 
                    // ADD     a,b 
                    // LD      (24h),a
                    
                    strBuilder.AppendLine($"\tLD hl, ({map[add.Left]})\t; LOAD {add.Left} from memory");
                    strBuilder.AppendLine($"\tLD a, l");
                    strBuilder.AppendLine($"\tLD hl, ({map[add.Right]})\t; LOAD {add.Left} from memory");
                    strBuilder.AppendLine($"\tLD b, l");
                    strBuilder.AppendLine($"\tADD a, b");
                    strBuilder.AppendLine($"\tLD ({map[add.Target]}), a \t; STORE into {add.Target}");

                    break;
                case Assign assign:
                    break;
                case AssignConstant assignConstant:
                    strBuilder.AppendLine($"\tLD hl, {map[assignConstant.Target]}");
                    strBuilder.AppendLine($"\tLD (hl), {assignConstant.Source}");
                    break;
                case Call call:
                    strBuilder.AppendLine($"\tCALL {call.Name}");
                    break;
                case Divide divide:
                    break;
                case EmptyReturn emptyReturn:
                    strBuilder.AppendLine("\tRET");
                    break;
                case Halt halt:
                    strBuilder.AppendLine("\tHALT");
                    break;
                case Label label:
                    strBuilder.Append($"{label.Name}");
                    break;
                case Multiply multiply:
                    break;
                case Return r:
                    break;
                case Subtract subtract:
                    break;
            }
        }

        return new GeneratedProgram(strBuilder.ToString(), new Dictionary<string, int>());
    }
}