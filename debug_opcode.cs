using System;
using CodeGeneration.Model.Classes;
using SomeCompiler.Z80.Core;

namespace DebugFactorial
{
    public class DebugOpCodeEmitter : OpCodeEmitter
    {
        public DebugOpCodeEmitter(Dictionary<Reference, MetaData> table) : base(table)
        {
        }
        
        public new string Set(int from, Register to)
        {
            Console.WriteLine($"DEBUG: OpCodeEmitter.Set called with from={from}, to={to.Name}");
            var result = base.Set(from, to);
            Console.WriteLine($"DEBUG: Result = '{result}'");
            return result;
        }
    }
}
