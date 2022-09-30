using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentAssertions.CSharpFunctionalExtensions;

namespace SomeCompiler.Z80.Tests
{
    public class Z80GeneratorTests
    {
        [Fact]
        public void Test1()
        {
            var input = "void main() { a = 1 + 2; }";
            var expected = 
                @"CALL    MAIN 
                HALT     
                MAIN:                
                LD      a,1 
                LD      b,2 
                ADD     a,b 
                RET";

            var compile = new CompilerFrontend();
            var result = compile
                .Emit(input)
                .Map(x => new Z80Generator().Generate(x));

            result.Should().BeSuccess().And.Subject.Value.Should().BeEquivalentTo(expected);
        }
    }
}