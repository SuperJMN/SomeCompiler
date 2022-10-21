using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;
using Xunit.Abstractions;

namespace SomeCompiler.Z80.Tests
{
    public class Z80GeneratorTests
    {
        private readonly ITestOutputHelper output;

        public Z80GeneratorTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact(Skip = "Not reliable. Can't assert.")]
        public void Simple_test()
        {
            var input = "void main() { a = 1; }";
            var expected =
@"CALL main
HALT
main:
LD hl, 30h
LD (hl), 1
RET";

            var compile = new CompilerFrontend();
            var result = compile
                .Emit(input)
                .Map(x => new Z80Generator().Generate(x));

            result.Tap(e => output.WriteLine(e));

            result.Should().BeSuccess()
                .And.Subject.Value.RemoveWhitespace()
                .Should()
                .BeEquivalentTo(expected.RemoveWhitespace());
        }
    }
}