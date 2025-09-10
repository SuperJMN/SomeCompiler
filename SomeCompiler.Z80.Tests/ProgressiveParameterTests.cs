using System;
using Xunit;

namespace SomeCompiler.Z80.Tests;

public class ProgressiveParameterTests
{
    [Fact]
    public void Test1_Function_with_no_parameters_returns_constant()
    {
        var src = @"int f() { return 42; } int main() { return f(); }";
        
        var result = Support.Z80E2E.RunHL(src);
        
        Console.WriteLine($"Result: {result} (expected: 42)");
        Assert.Equal(42, result);
    }
    
    [Fact] 
    public void Test2_Function_with_1_parameter_returns_it_unchanged()
    {
        var src = @"int f(int n) { return n; } int main() { return f(42); }";
        
        var result = Support.Z80E2E.RunHL(src);
        
        Console.WriteLine($"Result: {result} (expected: 42)");
        Assert.Equal(42, result);
    }
    
    [Fact]
    public void Test3_Function_with_1_parameter_adds_constant()
    {
        var src = @"int f(int n) { return n + 10; } int main() { return f(32); }";
        
        var result = Support.Z80E2E.RunHL(src);
        
        Console.WriteLine($"Result: {result} (expected: 42)");
        Assert.Equal(42, result);
    }
    
    [Fact]
    public void Test4_Function_with_2_parameters_returns_first()
    {
        var src = @"int f(int a, int b) { return a; } int main() { return f(42, 100); }";
        
        var result = Support.Z80E2E.RunHL(src);
        
        Console.WriteLine($"Result: {result} (expected: 42)");
        Assert.Equal(42, result);
    }
    
    [Fact]
    public void Test5_Function_with_2_parameters_returns_second()
    {
        var src = @"int f(int a, int b) { return b; } int main() { return f(100, 42); }";
        
        var result = Support.Z80E2E.RunHL(src);
        
        Console.WriteLine($"Result: {result} (expected: 42)");
        Assert.Equal(42, result);
    }
    
    [Fact]
    public void Test6_Function_with_3_parameters_returns_middle()
    {
        var src = @"int f(int a, int b, int c) { return b; } int main() { return f(10, 42, 100); }";
        
        var result = Support.Z80E2E.RunHL(src);
        
        Console.WriteLine($"Result: {result} (expected: 42)");
        Assert.Equal(42, result);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(42)]
    [InlineData(100)]
    public void Test7_Function_parameter_with_different_values(int value)
    {
        var src = $@"int f(int n) {{ return n; }} int main() {{ return f({value}); }}";
        
        var result = Support.Z80E2E.RunHL(src);
        
        Console.WriteLine($"Input: {value}, Result: {result}");
        Assert.Equal(value, result);
    }
}
