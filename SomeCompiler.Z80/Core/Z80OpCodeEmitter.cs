using CodeGeneration.Model.Classes;

namespace SomeCompiler.Z80.Core;

public class Z80OpCodeEmitter
{
    private readonly Dictionary<Reference, MetaData> table;

    public Z80OpCodeEmitter(Dictionary<Reference, MetaData> table)
    {
        this.table = table;
    }

    /// <summary>
    /// Loads from memory reference by <param name="from" /> into register <param name="to" />
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public string Set(Reference from, Register to)
    {
        return Tab + $"LD {to}, ({table[from].Address})\t; {to} = {table[from].Name}";
    }

    private const string Tab = "\t";

    public string Set(Register from, Reference to)
    {
        return Tab + $"LD ({table[to].Address}), {from}\t; {table[to].Name} = {from}";
    }

    public string Set(Register from, Register to)
    {
        return Tab + $"LD {to}, {from}\t; {to} = {from}";
    }

    public string Increment(Register register, Register increment)
    {
        return Tab + $"ADD {register}, {increment}\t; {register} += {increment}";
    }

    public string Return()
    {
        return Tab + "RET";
    }

    public string Set(int from, Register to)
    {
        return Tab + $"LD {to}, {from}\t; {to} = {from}";
    }

    public string Call(string label)
    {
        return Tab + $"CALL {label}";
    }

    public string Halt()
    {
        return Tab + "HALT";
    }
}