using Konamiman.Z80dotNet;

namespace SomeCompiler.Z80.Tests;

public class Z80State
{
    private readonly Z80Processor processor;

    public Z80State(Z80Processor processor)
    {
        this.processor = processor;
    }

    public IMemory Memory => processor.Memory;
}