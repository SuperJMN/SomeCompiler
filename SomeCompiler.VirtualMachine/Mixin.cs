using SomeCompiler.Generation.Intermediate.Model.Codes;

namespace SomeCompiler.VirtualMachine;

public static class Mixin
{
    public static IList<LabeledCode> AsLabeled(this IList<Code> program)
    {
        var prepend = new[] { Maybe.From((Label?) null) };

        var labels = prepend.Concat(program.Select(x => Maybe<Label>.From(x as Label)));
        var instructions = labels.Zip(program, (l, c) => new LabeledCode(l, c)).Where(x => x.Code is not Label);
        return instructions.ToList();
    }
}