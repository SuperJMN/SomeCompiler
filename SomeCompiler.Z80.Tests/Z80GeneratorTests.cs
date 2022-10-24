using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;

namespace SomeCompiler.Z80.Tests
{
    public class Z80GeneratorTests
    {
        [Fact]
        public void Simple_test()
        {
            var input = "void main() { a = 123; }";

            var result = Z80Runner.Run(input);

            result
                .Should().BeSuccess()
                .And.Subject.Value.Memory[48].Should().Be(123);
        }

        [Fact]
        public void Addition()
        {
            var input = "void main() { a = 1 + 2; }";

            var result = Z80Runner.Run(input);

            result
                .Should().BeSuccess()
                .And.Subject.Value.Memory[48].Should().Be(3);
        }
    }
}