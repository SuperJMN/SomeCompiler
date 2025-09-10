using System;
using Konamiman.Z80dotNet;
using SomeCompiler.Z80.Tests.Support;
using Xunit;

namespace SomeCompiler.Z80.Tests
{
    public class DebugOffsetTest
    {
        [Fact]
        public void Debug_parameter_offsets()
        {
            var src = @"int f(int n){ return n; } int main(){ return f(42); }";
            var (bin, entryPc) = Z80E2E.BuildBinaryAndEntryPc(src);
            
            var cpu = new Z80Processor();
            cpu.Reset();
            cpu.Memory.SetContents(0, bin);
            
            // Setup as in Z80E2E
            const ushort haltAddr = 0xF000;
            cpu.Memory[haltAddr] = 0x76; // HALT
            const ushort s0 = 0xFF00;
            cpu.Memory[s0] = (byte)(haltAddr & 0xFF);
            cpu.Memory[s0 + 1] = (byte)(haltAddr >> 8);
            cpu.Registers.SP = unchecked((short)s0);
            cpu.Registers.PC = entryPc;
            
            // Run until we reach the instruction that reads from IX
            bool foundIXRead = false;
            for (int i = 0; i < 10000 && !cpu.IsHalted; i++)
            {
                // Check if current instruction is LD L, (IX+2)
                var currentInstr = cpu.Memory[cpu.Registers.PC];
                var nextInstr = cpu.Memory[cpu.Registers.PC + 1];
                
                if (currentInstr == 0xDD && nextInstr == 0x6E) // LD L, (IX+d)
                {
                    var offset = (sbyte)cpu.Memory[cpu.Registers.PC + 2];
                    if (offset == 2)
                    {
                        foundIXRead = true;
                        var ix = (ushort)cpu.Registers.IX;
                        
                        Console.WriteLine($"At LD L, (IX+2): IX = 0x{ix:X4}");
                        for (int off = -8; off <= 8; off++)
                        {
                            var addr = (ushort)(ix + off);
                            var value = cpu.Memory[addr];
                            Console.WriteLine($"IX{off:+0;-0;+0} (0x{addr:X4}) = 0x{value:X2} ({value})");
                        }
                        break;
                    }
                }
                
                cpu.ExecuteNextInstruction();
            }
            
            Assert.True(foundIXRead, "Should have found the LD L, (IX+2) instruction");
        }
    }
}
