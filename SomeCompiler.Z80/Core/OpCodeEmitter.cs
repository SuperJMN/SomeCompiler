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
        var off = meta.Address; // Address field repurposed as frame offset (bytes). Negative for locals, positive for params.
        if (to.Name == "HL")
        {
            yield return Tab + $"LD L, {Disp(off)}\t; L = [{meta.Name}] low";
            yield return Tab + $"LD H, {Disp(off + 1)}\t; H = [{meta.Name}] high";
        }
        else
        {
            yield return Tab + $"LD {to}, {Disp(off)}\t; {to} = [{meta.Name}] low";
        }
    }

    /// <summary>
    /// Store a register into a frame slot (relative to IX).
    /// From HL, stores 16-bit (low, high). From 8-bit registers, stores low byte.
    /// </summary>
    public IEnumerable<string> Set(Register from, Reference to)
    {
        var meta = table[to];
        var off = meta.Address; // offset
        if (from.Name == "HL")
        {
            yield return Tab + $"LD {Disp(off)}, L\t; [{meta.Name}] low = L";
            yield return Tab + $"LD {Disp(off + 1)}, H\t; [{meta.Name}] high = H";
        }
        else
        {
            yield return Tab + $"LD {Disp(off)}, {from}\t; [{meta.Name}] low = {from}";
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

    public string Set(int from, Register to)
    {
        return Tab + $"LD {to}, {from}\t; {to} = {from}";
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
}
