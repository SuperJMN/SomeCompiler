using System;
using SomeCompiler.Z80.Tests.Support;

public class DebugZ80E2E
{
    public static void Main()
    {
        try
        {
            Console.WriteLine("Testing: int main() { return 7; }");
            var result = Z80E2E.RunHL("int main() { return 7; }", maxSteps: 100);
            Console.WriteLine($"Result: {result}");
            Console.WriteLine($"Expected: 7");
            Console.WriteLine($"Success: {result == 7}");
            
            Console.WriteLine("\nTesting: int main() { return 42; }");
            var result2 = Z80E2E.RunHL("int main() { return 42; }", maxSteps: 100);
            Console.WriteLine($"Result: {result2}");
            Console.WriteLine($"Expected: 42");
            Console.WriteLine($"Success: {result2 == 42}");
            
            Console.WriteLine("\nTesting: int main() { return 0; }");
            var result3 = Z80E2E.RunHL("int main() { return 0; }", maxSteps: 100);
            Console.WriteLine($"Result: {result3}");
            Console.WriteLine($"Expected: 0");
            Console.WriteLine($"Success: {result3 == 0}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex}");
        }
    }
}
