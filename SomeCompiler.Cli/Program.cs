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

void PrintRunResult(int hl)
{
    PrintSection("Run result (HL):", hl.ToString());
}

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

Result<int> RunAsm(SomeCompiler.Z80.Core.GeneratedProgram asm)
{
    // Assemble with Sixty502DotNet Z80Assembler and run on Konamiman.Z80dotNet
    var assembler = new Sixty502DotNet.Z80Assembler();
    var assembled = assembler.Assemble(asm.Assembly);
    if (assembled.IsFailure) return Result.Failure<int>(assembled.Error);

    var bin = assembled.Value.ProgramBinary;
    // Determine entry PC: try to find "main:" label in debug info
    ushort entryPc = (ushort)assembled.Value.DebugInfo
        .Where(d => (d.LineText?.Trim() ?? string.Empty) == "main:")
        .Select(d => d.ProgramCounter)
        .DefaultIfEmpty(0)
        .First();

    if (entryPc == 0)
    {
        // Fallback: scan assembly text for main: and pick first instruction after
        var lines = asm.Assembly.Split('\n');
        for (int i = 0; i < lines.Length && entryPc == 0; i++)
        {
            if (lines[i].Trim() == "main:")
            {
                for (int j = i + 1; j < lines.Length; j++)
                {
                    var next = lines[j].Trim();
                    if (!string.IsNullOrEmpty(next) && !next.EndsWith(':'))
                    {
                        // Pick first instruction after label; use its first matching debug entry with highest PC
                        var instruction = next.Split('\t')[0];
                        var match = assembled.Value.DebugInfo
                            .Where(d => d.LineText?.Trim().StartsWith(instruction) == true)
                            .OrderBy(d => d.ProgramCounter)
                            .LastOrDefault();
                        if (match != null) entryPc = (ushort)match.ProgramCounter;
                        break;
                    }
                }
            }
        }
    }

    var cpu = new Konamiman.Z80dotNet.Z80Processor();
    cpu.Reset();
    cpu.Memory.SetContents(0, bin);

    const ushort haltAddr = 0xF000;
    cpu.Memory[haltAddr] = 0x76; // HALT
    const ushort s0 = 0xFF00;
    cpu.Memory[s0] = (byte)(haltAddr & 0xFF);
    cpu.Memory[s0 + 1] = (byte)(haltAddr >> 8);
    cpu.Registers.SP = unchecked((short)s0);

    cpu.Registers.PC = entryPc;

    for (int i = 0; i < 20000 && !cpu.IsHalted; i++)
    {
        cpu.ExecuteNextInstruction();
    }

    if (!cpu.IsHalted) return Result.Failure<int>("Z80 execution didn't reach HALT within step bound.");

    int hl = (cpu.Registers.H << 8) | (cpu.Registers.L & 0xFF);
    return Result.Success(hl);
}

void PrintIR(SomeCompiler.Generation.Intermediate.Model.IntermediateCodeProgram ir)
{
    var text = SomeCompiler.Generation.Intermediate.Model.Visitors.PrettyPrinterVisitor.Print(ir);
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
    .Bind(RunAsm)
    .Tap(PrintRunResult)
    .Match(_ => PrintSuccess(), PrintError);
