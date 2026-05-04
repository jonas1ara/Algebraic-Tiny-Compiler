# Algebraic Tiny Compiler (ATC) - F# Edition

A lightweight compiler for algebraic expressions written in F# to understand compiler design concepts.

## Purpose

This project demonstrates core compiler concepts through building an algebraic expression compiler:
- **Tokenization**: Breaking input into meaningful tokens
- **Parsing**: Building Abstract Syntax Trees (AST) with recursive descent parsing
- **Simplification**: Polynomial expansion and like-term combination
- **Code Generation**: Producing optimized low-level code
- **Equation Solving**: Solving linear and quadratic equations


## Quick Start

```bash
# Build the project
dotnet build src/

# Run the interactive compiler
dotnet run --project src/

# Example session
Enter expression: 2x + 3 = 5
Solutions:
  x1 = 1.000000
```

## Architecture

```
Input String
    ↓
Tokenizer → Tokens
    ↓
Parser → AST (Expr)
    ↓
Simplifier → Polynomial
    ↓
Solver / Codegen → Solutions / Assembly Code
```

### Core Modules (in `src/`)

| Module | Purpose |
|--------|---------|
| **AlgebraTokenizer.fs** | Lexical analysis - converts strings to tokens |
| **AlgebraParser.fs** | Syntax analysis - recursive descent parser with operator precedence |
| **AlgebraTypes.fs** | Type definitions for AST, polynomials, equations |
| **AlgebraSimplifier.fs** | Semantic analysis - expands and simplifies expressions |
| **AlgebraSolver.fs** | Linear and quadratic equation solving |
| **AlgebraCodegen.fs** | Code generation - produces assembly-like output |

## Supported Syntax

### Expressions
```
2x + 5
x^2 - 3x + 2
(x + 1)(x - 2)
```

### Equations
```
2x + 3 = 5
x^2 - 5x + 6 = 0
2x^2 + 3x - 5 = 0
```

### Features
- Variables and coefficients
- Powers (x^n)
- Parentheses grouping
- Implicit multiplication (2x, 2(x+1))
- Like-term combining
- Linear equation solving
- Quadratic equation solving (with discriminant)

## Compilation Modes

### JIT (Default)
```bash
dotnet build src/
dotnet run --project src/
# Startup: ~200ms, Memory: ~100MB, GC pauses: yes
```

### AoT (Optimized)
```bash
dotnet publish src/ -c Release -r win-x64 --self-contained /p:PublishAot=true
./bin/Release/net10.0/win-x64/publish/AlgebraicTinyCompiler.exe
# Startup: ~5ms, Memory: ~15MB, GC pauses: none
```

## Learning Outcomes

After studying this compiler, you'll understand:

1. **Lexical Analysis**: How source code is tokenized
2. **Syntax Analysis**: How recursive descent parsing works with operator precedence
3. **AST Construction**: Building intermediate representations
4. **Semantic Analysis**: Type checking and expression simplification
5. **Code Generation**: Producing optimized output code
6. **Equation Solving**: Algorithmic problem solving within a compiler
7. **JIT vs AoT**: Compilation strategy impacts on performance and memory
8. **F# Functional Programming**: Discriminated unions, pattern matching, recursion

## Technical Highlights

- **F# Discriminated Unions** for type-safe AST representation
- **Recursive Descent Parser** with operator precedence handling
- **Polynomial Representation** as list of (coefficient, variable, power) terms
- **Automatic Simplification** of like terms via grouping
- **Virtual Register** allocation in code generation
- **AoT Compilation** for 40x faster startup and minimal memory

## Project Structure

```
Algebraic-Tiny-Compiler/
├── src/
│   ├── Tokenizer.fs           # Simple arithmetic tokenizer (reference)
│   ├── Parser.fs              # Simple arithmetic parser (reference)
│   ├── Assembly.fs            # Code generation (reference)
│   ├── AlgebraTypes.fs        # Core compiler types
│   ├── AlgebraTokenizer.fs    # Algebraic expression tokenizer
│   ├── AlgebraParser.fs       # Recursive descent parser
│   ├── AlgebraSimplifier.fs   # Expression simplification
│   ├── AlgebraSolver.fs       # Equation solver
│   ├── AlgebraCodegen.fs      # Code generator
│   ├── Program.fs             # Interactive CLI
│   └── AlgebraicTinyCompiler.fsproj
├── ALGEBRA_COMPILER.md        # Detailed compilation guide
├── AOT_COMPILATION_GUIDE.md   # Performance & deployment
├── STRUCTURE_FSHARP.md        # F# architecture & types
├── README.md                  # This file
└── 2026-05-03-understanding-compilers-through-algebra.md  # Educational blog post
```

## References

- [Blog Post: Understanding Compilers Through Algebra](2026-05-03-understanding-compilers-through-algebra.md)
- [F# Language Reference](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/)
- [F# Discriminated Unions](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/discriminated-unions)
- [F# Pattern Matching](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/pattern-matching)
- Original C implementation: [arithmetic-compiler](https://github.com/TrisH0x2A/project-box/tree/main/arithmetic-compiler)
- Compiler textbook: "Engineering a Compiler" by Cooper & Torczon
- Online resource: [Crafting Interpreters](https://craftinginterpreters.com/)

