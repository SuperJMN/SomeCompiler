using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace SomeCompiler.SemanticAnalysis.Tests;

public class SemanticSampleSuite
{
    public static IEnumerable<object[]> Cases()
        => Directory.EnumerateFiles(Path.Combine("Samples"), "*.c", SearchOption.AllDirectories)
                     .OrderBy(p => p)
                     .Select(p => new object[] { p });

    [Theory]
    [MemberData(nameof(Cases))]
    public async Task Golden_samples(string path)
    {
        var source = await File.ReadAllTextAsync(path);
        var analyzed = SemanticTestDriver.Analyze(source);
        Assert.True(analyzed.IsSuccess, analyzed.IsFailure ? analyzed.Error : "");
        var text = SemanticSnapshotPrinter.Print(analyzed.Value);

        var name = Path.GetFileNameWithoutExtension(path);
        var dir = Path.GetDirectoryName(path)!;
        await Verifier.Verify(text)
            .UseDirectory(dir)
            .UseMethodName(name);
    }
}
