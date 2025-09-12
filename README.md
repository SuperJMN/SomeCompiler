# RetroSharp

RetroSharp is a modern C#-like language that compiles to 8-bit architectures, created for "The Joy of Learning®".

I'm making this in my free time to learn about compilers and some old-school topics. It combines the familiar syntax of C# with the nostalgic charm of retro computing. It's supposed to make me happier, but more often that not, it's making me get bald faster 🤣

## How does it work?

RetroSharp uses a multi-stage compilation pipeline:

1. **Parser**: Uses ANTLR4 to parse RetroSharp source code into an AST
2. **Semantic Analysis**: Validates types, scopes, and semantics
3. **Intermediate Code Generation**: Produces platform-agnostic 3-address code (IL)
4. **Backend**: Translates IL to target architecture (currently Zilog Z80)

The benefit of this architecture is that the IL is generic enough to target virtually any platform by just writing a new backend.

## What can it do?

Right now, RetroSharp can compile simple programs with:
- Basic arithmetic and logic operations
- Variables and assignments
- Function calls
- Control flow (if/else, loops)
- Multiple data types (int, char, byte, u8, i8, u16, i16, bool)
- Pointers with `ptr<T>` syntax

Example program:
```csharp
int main() 
{ 
    return 2 * 3 * 4; 
}
```

## Which platforms does it compile for?

Currently, RetroSharp targets the **Zilog Z80** processor - one of the most iconic 8-bit CPUs of all time! The Z80 powered legendary systems like:

- Nintendo Game Boy
- Amstrad CPC
- MSX computers
- TRS-80
- And many arcade machines

The modular design makes it relatively straightforward to add support for other 8-bit processors like the 6502, 8080, or even modern microcontrollers.

## Installation

RetroSharp is distributed as a .NET tool:

```bash
dotnet tool install --global RetroSharp
```

Then use it to compile your programs:

```bash
retroSharp myprogram.rs
```
