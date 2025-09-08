# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

Repository type: .NET 8 multi-project solution (C#) for a small C-like compiler that emits an intermediate 3-address code and targets Zilog Z80.

Common commands
- Restore and build (all projects)
  - Debug: dotnet build SomeCompiler.sln -c Debug
  - Release: dotnet build SomeCompiler.sln -c Release
- Run all tests (xUnit)
  - dotnet test SomeCompiler.sln -c Debug --no-build
- Run tests for a single project
  - dotnet test SomeCompiler.Parser.Tests/SomeCompiler.Parser.Tests.csproj -c Debug --no-build
- Run a single test
  - By fully-qualified name: dotnet test --filter "FullyQualifiedName~Namespace.ClassName.MethodName"
  - By class: dotnet test --filter "ClassName=Namespace.ClassName"
- Code formatting/linting (uses .NET analyzers/formatting)
  - Fix: dotnet format SomeCompiler.sln
  - Check only (CI-friendly): dotnet format SomeCompiler.sln --verify-no-changes
- Run the CLI (compile a source file and print IL and Z80 assembly)
  - dotnet run --project SomeCompiler.Cli -- path/to/source.c
- Optional: collect coverage (coverlet collector is referenced in test projects)
  - dotnet test SomeCompiler.sln --collect "XPlat Code Coverage"

Prerequisites
- .NET SDK 8.x installed and available on PATH

High-level architecture
The solution is structured as a classic compiler pipeline with clear stage separation and an additional VM and backend for Z80.

- Frontend (Parsing)
  - SomeCompiler.Parser.Antlr4.v2: Antlr4-based parser. The grammar (SomeLanguage.g4) is processed via Antlr4BuildTasks at build time to generate lexer/parser code. A small Program.cs is included for quick parsing experiments.
  - SomeCompiler.Parser.Antlr4: Earlier Antlr4-based parser variant kept for reference/evolution.
  - SomeCompiler.Parser.Model: Shared parser/AST model abstractions (depends on SomeCompiler.Core).

- Core and shared domain
  - SomeCompiler.Core: Core abstractions used across stages (operators, precedence, binary nodes/trees, helpers). The repo adopts CSharpFunctionalExtensions and Zafiro.Core for functional and utility patterns.

- Semantic analysis
  - SomeCompiler.SemanticAnalysis: Consumes parser output and enriches it with semantic information. Downstream of Core and Parser.

- Intermediate representation (IR) and code generation
  - SomeCompiler.Generation.Intermediate: Defines the intermediate 3-address code model used across the compiler (e.g., Add, Subtract, Multiply, Divide, And, Or, Assign, Call, Return, Label, Halt). Contains Fragment and supporting types and a CodeFormatter to render IL.
  - SomeCompiler (root project): Hosts the compiler orchestration and (historically) IL generation glue. It references the Antlr4 v2 parser and uses functional patterns to produce an IntermediateCodeProgram. Utility types for scope handling live under SomeCompiler/Utils.

- Virtual machine (IL execution)
  - SomeCompiler.VirtualMachine: Provides a simple VM layer with commands mirroring the IR operations (e.g., AddCommand, AssignConstantCommand, CallCommand, ReturnCommand, HaltCommand). Useful for validating IR execution behavior independently of a hardware backend.

- Backend (Z80)
  - SomeCompiler.Z80: Translates the IR into Z80 assembly and related artifacts for the Zilog Z80 target.
  - SomeCompiler.Cli: Thin command-line app that ties the pipeline together: reads a C-like source file, compiles to IR, prints IR, generates Z80 assembly, and prints it. Exit reporting is via stderr for success/error messages.

- External projects included in the solution
  - Z80DotNet/*: Z80 processor simulator and tests (e.g., Zexall). Used as a real-world Z80 reference/executor for validation.
  - 6502DotNet/Sixty502DotNet/*: Separate external assembler/runtime referenced by the solution (not part of the compiler pipeline itself).

Development notes
- Grammar changes: Building SomeCompiler.Parser.Antlr4.v2 will regenerate parser sources via Antlr4BuildTasks. A full solution build will handle this automatically; you can also build that project directly if iterating on the grammar.
- Project targets: All projects target net8.0 with nullable and implicit usings enabled.
- Test framework: xUnit across test projects (SomeCompiler.*.Tests). Prefer running tests per project when iterating quickly.

