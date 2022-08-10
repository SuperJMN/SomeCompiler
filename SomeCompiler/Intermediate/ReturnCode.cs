using CSharpFunctionalExtensions;

namespace SomeCompiler.Intermediate;

internal class ReturnCode : IntermediateCode
{
    public Maybe<object> Constant { get; }

    public ReturnCode(Maybe<object> constant)
    {
        Constant = constant;
    }

    public override string ToString() => string.Join(" ", new[] { "Return", Constant.GetValueOrDefault() });
}