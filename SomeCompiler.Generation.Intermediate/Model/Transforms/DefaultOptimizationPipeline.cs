using CSharpFunctionalExtensions;

namespace SomeCompiler.Generation.Intermediate.Model.Transforms;

public static class DefaultOptimizationPipeline
{
public static Result<IntermediateCodeProgram> Apply(IntermediateCodeProgram program)
{
    // Chain: Constant Folding -> Copy Propagation -> Dead Code Elimination
    var folding = new ConstantFoldingVisitor();
    var copyProp = new CopyPropagationVisitor();
    var dce = new DeadCodeEliminationVisitor();

    return folding.Run(program)
        .Bind(copyProp.Run)
        .Bind(dce.Run);
}
}
