using CSharpFunctionalExtensions;

namespace RetroSharp.Generation.Intermediate.Model.Transforms;

public interface IIntermediateTransform
{
    Result<IntermediateCodeProgram> Run(IntermediateCodeProgram input);
}
