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
        
        [Fact]
        public void Simple_test()
        {
            var input = "int main() { return 123; }";

            var result = new Z80Runner(output).Run(input);

            result
                .Should().BeSuccess()
                .And.Subject.Value.Registers.L.Should().Be(123);
        }

        [Fact]
        public void Addition()
        {
            var input = "int main() { return 1 + 2; }";

            var result = new Z80Runner(output).Run(input);

            result
                .Should().BeSuccess()
                .And.Subject.Value.Registers.L.Should().Be(3);
        }
        
        [Fact]
        public void Multiplication()
        {
            var input = "int main() { return 2 * 3; }";

            var result = new Z80Runner(output).Run(input);

            result
                .Should().BeSuccess()
                .And.Subject.Value.Registers.L.Should().Be(6);
        }
        
        [Fact]
        public void Multiplication_twice()
        {
            var input = "int main() { return 2 * 3 * 4; }";

            var result = new Z80Runner(output).Run(input);

            result
                .Should().BeSuccess()
                .And.Subject.Value.Registers.L.Should().Be(24);
        }
    }
}