using System;
using SomeCompiler.Z80.Tests.Support;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Test 1: Basic LD HL, 3 ===");
        TestBasicLoad();
        
        Console.WriteLine();
        Console.WriteLine("=== Test 2: Arithmetic 3 - 1 ===");
        TestArithmetic();
        
        Console.WriteLine();
        Console.WriteLine("=== Test 3: Simple function call ===");
        TestSimpleFunctionCall();
        
        Console.WriteLine();
        Console.WriteLine("=== Test 4: Function with literal return ===");
        TestFunctionLiteralReturn();
        
        Console.WriteLine();
        Console.WriteLine("=== Test 5: Parameter access debug ===");
        TestParameterAccess();
    }
    
    static void TestBasicLoad()
    {
        string src = "int main() { return 3; }";
        var result = Z80E2E.RunHL(src);
        Console.WriteLine($"Expected: 3, Actual: {result}");
        if (result == 3) Console.WriteLine("✓ PASS");
        else Console.WriteLine("✗ FAIL");
    }
    
    static void TestArithmetic()
    {
        string src = "int main() { return 3 - 1; }";
        var result = Z80E2E.RunHL(src);
        Console.WriteLine($"Expected: 2, Actual: {result}");
        if (result == 2) Console.WriteLine("✓ PASS");
        else Console.WriteLine("✗ FAIL");
    }
    
    static void TestSimpleFunctionCall()
    {
        string src = @"
            int identity(int n) {
                return n;
            }
            
            int main() {
                return identity(3);
            }";
        var result = Z80E2E.RunHL(src);
        Console.WriteLine($"Expected: 3, Actual: {result}");
        if (result == 3) Console.WriteLine("✓ PASS");
        else Console.WriteLine("✗ FAIL");
    }
    
    static void TestFunctionLiteralReturn()
    {
        string src = @"
            int getConstant(int n) {
                return 42;
            }
            
            int main() {
                return getConstant(3);
            }";
        var result = Z80E2E.RunHL(src);
        Console.WriteLine($"Expected: 42, Actual: {result}");
        if (result == 42) Console.WriteLine("✓ PASS");
        else Console.WriteLine("✗ FAIL");
    }
    
    static void TestParameterAccess()
    {
        string src = @"
            int addOne(int n) {
                return n + 1;
            }
            
            int main() {
                return addOne(5);
            }";
        var result = Z80E2E.RunHL(src);
        Console.WriteLine($"Expected: 6, Actual: {result}");
        if (result == 6) Console.WriteLine("✓ PASS");
        else Console.WriteLine("✗ FAIL");
    }
}
