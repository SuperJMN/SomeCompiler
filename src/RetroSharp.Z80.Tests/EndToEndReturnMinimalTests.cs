using Xunit;
using RetroSharp.Z80.Tests.Support;

namespace RetroSharp.Z80.Tests;

public class EndToEndReturnMinimalTests
{
    [Theory]
    [InlineData("int main() { return 0; }", 0)]
    [InlineData("int main() { return 7; }", 7)]
    [InlineData("int main() { return 42; }", 42)]
    public void Main_returns_expected(string source, int expected)
    {
        var actual = Z80E2E.RunHL(source);
        Assert.Equal(expected, actual);
    }
}

