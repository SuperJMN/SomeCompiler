void PrintSection(string header, string body)
{
    Console.WriteLine(header);
    Console.WriteLine(body);
    Console.WriteLine();
}

string ReadInputFile(string s) => File.ReadAllText(s);

void PrintSuccess() => Console.Error.WriteLine("Success");
void PrintError(string s) => Console.Error.WriteLine(s);
void PrintAsm(SomeCompiler.Z80.Core.GeneratedProgram gp) => PrintSection("Assembly:", gp.Assembly);

void PrintSource(string source) => PrintSection("Source:", source);

void PrintDiagnostics(IEnumerable<string> diagnostics)
{
    var text = diagnostics.Any() ? string.Join("\n", diagnostics) : "None";
    PrintSection("Diagnostics:", text);
}

Result<string> Parse(string source)
{
    var parser = new SomeCompiler.Parser.SomeParser();
    var parsed = parser.Parse(source);
    return parsed.Map(_ => source).MapError(err => err);
}

Result<SomeCompiler.SemanticAnalysis.AnalyzeResult<SomeCompiler.SemanticAnalysis.SemanticNode>> Analyze(string source)
{
    var parser = new SomeCompiler.Parser.SomeParser();
    var parse = parser.Parse(source);
    if (parse.IsFailure) return Result.Failure<SomeCompiler.SemanticAnalysis.AnalyzeResult<SomeCompiler.SemanticAnalysis.SemanticNode>>(parse.Error);

    var analyzer = new SomeCompiler.SemanticAnalysis.SemanticAnalyzer();
    var analyzed = analyzer.Analyze(parse.Value);
    return Result.Success(analyzed);
}

Result<SomeCompiler.Generation.Intermediate.Model.IntermediateCodeProgram> GenerateIR(SomeCompiler.SemanticAnalysis.AnalyzeResult<SomeCompiler.SemanticAnalysis.SemanticNode> analyzed)
{
    var gen = new SomeCompiler.Generation.Intermediate.V2IntermediateCodeGenerator();
    var programNode = (SomeCompiler.SemanticAnalysis.ProgramNode)analyzed.Node;
    var ir = gen.Generate(programNode);
    return Result.Success(ir);
}

Result<SomeCompiler.Generation.Intermediate.Model.IntermediateCodeProgram> OptimizeIR(SomeCompiler.Generation.Intermediate.Model.IntermediateCodeProgram ir)
{
    return SomeCompiler.Generation.Intermediate.Model.Transforms.DefaultOptimizationPipeline
        .Apply(ir);
}

Result<SomeCompiler.Z80.Core.GeneratedProgram> GenerateAsm(SomeCompiler.Generation.Intermediate.Model.IntermediateCodeProgram ir)
{
    var z80 = new SomeCompiler.Z80.Z80Generator();
    return z80.Generate(ir);
}

void PrintIR(SomeCompiler.Generation.Intermediate.Model.IntermediateCodeProgram ir)
{
    var text = string.Join("\n", SomeCompiler.Generation.Intermediate.IntermediateProgramExtensions.ToTextFormatContent(ir));
    PrintSection("Intermediate code:", text);
}

if (args.Length < 1)
{
    Console.Error.WriteLine("No source file has been specified");
    return;
}

var path = args[0];

Result
    .Try(() => ReadInputFile(path))
    .Tap(PrintSource)
    .Bind(Analyze)
    .Tap(result => PrintDiagnostics(result.Node.AllErrors))
.Bind(GenerateIR)
    .Bind(OptimizeIR)
    .Tap(PrintIR)
    .Bind(GenerateAsm)
    .Tap(PrintAsm)
    .Match(_ => PrintSuccess(), PrintError);
