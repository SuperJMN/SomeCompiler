using CSharpFunctionalExtensions;

namespace SomeCompiler.Generation.Intermediate.Model.Transforms;

public interface IIntermediateTransform
{
    Result<IntermediateCodeProgram> Run(IntermediateCodeProgram input);
}
