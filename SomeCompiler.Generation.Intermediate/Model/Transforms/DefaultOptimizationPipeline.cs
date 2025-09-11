using CSharpFunctionalExtensions;

namespace SomeCompiler.Generation.Intermediate.Model.Transforms;

public static class DefaultOptimizationPipeline
{
    public static Result<IntermediateCodeProgram> Apply(IntermediateCodeProgram program)
    {
        // For now: single pass constant folding. Later we can chain more transforms.
        var folding = new ConstantFoldingVisitor();
        return folding.Run(program);
    }
}
