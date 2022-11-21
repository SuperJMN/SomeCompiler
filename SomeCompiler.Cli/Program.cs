void PrintSource(string source)
{
    PrintSection("Source code:", source);
}

void PrintSection(string header, string body)
{
    Console.WriteLine(header);
    Console.WriteLine(body);
    Console.WriteLine();
}

string ReadInputFile(string s)
{
    return File.ReadAllText(s);
}

Result<GeneratedProgram> Generate(IntermediateCodeProgram intermediateCodeProgram)
{
    return new Z80Generator().Generate(intermediateCodeProgram);
}

void PrintSuccess()
{
    Console.Error.WriteLine("Success");
}

void PrintError(string s)
{
    Console.Error.WriteLine(s);
}

void PrintAssembly(GeneratedProgram generatedProgram)
{
    PrintSection("Z80 assembly:", generatedProgram.Assembly);
}

void PrintIL(IntermediateCodeProgram intermediateCodeProgram)
{
    PrintSection("Intermediate code:", intermediateCodeProgram.ToCodeFormatContent().JoinWithLines());
}

Result<IntermediateCodeProgram> Compile(string s)
{
    return new Compiler().Emit(s).MapError(errors => errors.JoinWithLines());
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
    .Bind(Compile)
    .Tap(PrintIL)
    .Bind(Generate)
    .Tap(PrintAssembly)
    .Match(_ => PrintSuccess(), PrintError);
    