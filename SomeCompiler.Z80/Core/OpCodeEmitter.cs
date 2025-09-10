using CodeGeneration.Model.Classes;

namespace SomeCompiler.Z80.Core;

public class OpCodeEmitter
{
    private readonly Dictionary<Reference, MetaData> table;

    public OpCodeEmitter(Dictionary<Reference, MetaData> table)
    {
        this.table = table;
    }

    private const string Tab = "\t";

    private static string Disp(int off)
    {
        return off < 0 ? $"(IX{off})" : $"(IX+{off}" + ")"; // format (IX-2) or (IX+2)
    }

    /// <summary>
    /// Load from a frame slot (relative to IX) into a register.
    /// For HL, loads 16-bit (low then high). For 8-bit registers, loads the low byte.
    /// </summary>
    public IEnumerable<string> Set(Reference from, Register to)
    {
        var meta = table[from];
        if (to.Name == "HL")
        {
            yield return Tab + $"LD L, {Disp(meta.LowOffset)}\t; L = [{meta.Name}] low";
            yield return Tab + $"LD H, {Disp(meta.HighOffset)}\t; H = [{meta.Name}] high";
        }
        else
        {
            yield return Tab + $"LD {to}, {Disp(meta.LowOffset)}\t; {to} = [{meta.Name}] low";
        }
    }

    /// <summary>
    /// Store a register into a frame slot (relative to IX).
    /// From HL, stores 16-bit (low, high). From 8-bit registers, stores low byte.
    /// </summary>
    public IEnumerable<string> Set(Register from, Reference to)
    {
        var meta = table[to];
        if (from.Name == "HL")
        {
            yield return Tab + $"LD {Disp(meta.LowOffset)}, L\t; [{meta.Name}] low = L";
            yield return Tab + $"LD {Disp(meta.HighOffset)}, H\t; [{meta.Name}] high = H";
        }
        else
        {
            yield return Tab + $"LD {Disp(meta.LowOffset)}, {from}\t; [{meta.Name}] low = {from}";
        }
    }

    public string Set(Register from, Register to)
    {
        return Tab + $"LD {to}, {from}\t; {to} = {from}";
    }

    public string Increment(Register register, Register increment)
    {
        return Tab + $"ADD {register}, {increment}\t; {register} += {increment}";
    }

    public IEnumerable<string> EpilogueAndReturn()
    {
        yield return Tab + "LD SP, IX";
        yield return Tab + "POP IX";
        yield return Tab + "RET";
    }

    public IEnumerable<string> AcademicEpilogueAndReturn(int frameSize)
    {
        // Academic convention: callee with parameters didn't modify IX
        // Just restore SP and return (no IX manipulation)
        if (frameSize > 0)
        {
            yield return Tab + $"LD HL, {frameSize}";
            yield return Tab + "ADD HL, SP";
            yield return Tab + "LD SP, HL";
        }
        yield return Tab + "RET";
    }

    public IEnumerable<string> EpilogueAndReturnWithParamCleanup(int paramCount)
    {
        yield return Tab + "LD SP, IX";
        yield return Tab + "POP IX";
        // Clean parameters from stack (each parameter is 2 bytes)
        if (paramCount > 0)
        {
            yield return Tab + $"LD HL, {paramCount * 2}";
            yield return Tab + "ADD HL, SP";
            yield return Tab + "LD SP, HL";
        }
        yield return Tab + "RET";
    }

    public string Set(int from, Register to)
    {
        return Tab + $"LD {to}, {from}\t; {to} = {from}";
    }

    public bool HasReference(CodeGeneration.Model.Classes.Reference reference)
    {
        return table.ContainsKey(reference);
    }

    public string Call(string label)
    {
        return Tab + $"CALL {label}";
    }

    public IEnumerable<string> AdjustSP(int bytes)
    {
        if (bytes == 0) yield break;
        yield return Tab + $"LD HL, {bytes}";
        yield return Tab + "ADD HL, SP";
        yield return Tab + "LD SP, HL";
    }

    public IEnumerable<string> AdjustSPPreserveHL(int bytes)
    {
        if (bytes == 0) yield break;
        yield return Tab + "LD D, H";
        yield return Tab + "LD E, L";
        yield return Tab + $"LD HL, {bytes}";
        yield return Tab + "ADD HL, SP";
        yield return Tab + "LD SP, HL";
        yield return Tab + "LD H, D";
        yield return Tab + "LD L, E";
    }

    public string Push(Register reg)
    {
        return Tab + $"PUSH {reg}";
    }

    public string Jump(string label) => Tab + $"JP {label}";

    public IEnumerable<string> BranchIfHLZero(string label)
    {
        yield return Tab + "LD A, L";
        yield return Tab + "OR H";
        yield return Tab + $"JP Z, {label}";
    }

    public IEnumerable<string> BranchIfHLNotZero(string label)
    {
        yield return Tab + "LD A, L";
        yield return Tab + "OR H";
        yield return Tab + $"JP NZ, {label}";
    }

    public string Halt()
    {
        return Tab + "HALT";
    }

    // Clears carry flag (and A=0). Useful before 16-bit SBC/ADC.
    public string XorA()
    {
        return Tab + "XOR A";
    }

    // 16-bit subtract: HL = HL - DE - Carry
    public string SbcHlDe()
    {
        return Tab + "SBC HL, DE";
    }
}
