# F# Architecture

## Project Structure

```
src/
├── Tokenizer.fs              # Lexical analysis (arithmetic subset)
├── Parser.fs                 # Syntax analysis (arithmetic subset)
├── Assembly.fs               # Code generation (arithmetic subset)
├── AlgebraTypes.fs           # Core type definitions
├── AlgebraTokenizer.fs       # Extended tokenizer for algebra
├── AlgebraParser.fs          # Recursive descent parser
├── AlgebraSimplifier.fs      # Polynomial expansion & simplification
├── AlgebraSolver.fs          # Linear & quadratic equation solving
├── AlgebraCodegen.fs         # Virtual register code generation
├── Program.fs                # Interactive CLI
└── AlgebraicTinyCompiler.fsproj
```

## Module Hierarchy

```
Program.fs (entry point)
    ↓
┌─────────────────────────────────────┐
│  AlgebraTypes                       │
│  (Expr, Term, Polynomial, Equation) │
└─────────────────────────────────────┘
    ↓
AlgebraTokenizer.fs → AlgebraParser.fs → AlgebraSimplifier.fs
                                             ↓
                                    ┌────────────────────┐
                                    ├─ AlgebraSolver.fs  │
                                    ├─ AlgebraCodegen.fs │
                                    └────────────────────┘
```

## Type System

### Core Expressions

```fsharp
type Expr =
    | Num of float           // Numeric literals: 3.14
    | Var of string          // Variables: x, y
    | Binop of string * Expr * Expr  // Binary operations: x + y
    | Power of Expr * int    // Powers: x^2
    | Paren of Expr          // Parenthesized: (x + 1)
    | UnaryOp of string * Expr       // Unary: -x
```

### Polynomial Representation

```fsharp
type Term = {
    Coefficient: float
    Variable: string
    Power: int
}

type Polynomial = Term list
// Example: 2x² + 3x - 5
// Represents as: [Term(2.0, "x", 2); Term(3.0, "x", 1); Term(-5.0, "", 0)]
```

### Equations

```fsharp
type Equation = {
    Left: Expr
    Right: Expr
}
```

## Compilation Pipeline

```
User Input: "2x^2 + 3x - 5 = 0"
    ↓
[TOKENIZE] AlgebraTokenizer.tokenize
    → [Number(2); Var(x); Power(^); Number(2); ...]
    ↓
[PARSE] AlgebraParser.parseEquation
    → Equation { Left: Power(Binop(...)); Right: Num(0.0) }
    ↓
[SIMPLIFY] AlgebraSimplifier.expandExpression
    → Polynomial [Term(2.0, "x", 2); Term(3.0, "x", 1); Term(-5.0, "", 0)]
    ↓
[SOLVE] AlgebraSolver.solveEquation
    → [1.0; -2.5]
    ↓
[CODEGEN] AlgebraCodegen.generateEquationCode
    → ["LOAD 2"; "LOAD_VAR x"; "POW R0 R1 2"; ...]
```

## Key Functions

### AlgebraTokenizer

```fsharp
val tokenize : string -> Token list
// Breaks input into meaningful tokens
// Recognizes: numbers, variables, operators (+, -, *, /, ^, =, etc.)
```

### AlgebraParser

```fsharp
val parseEquation : string -> Equation option
val parseExpressionString : string -> Expr option

// Recursive descent with operator precedence:
// 1. parseExpression  (+ -)
// 2. parseTerm        (* /)
// 3. parseFactor      (^)
// 4. parsePrimary     (number, variable, parentheses)
```

### AlgebraSimplifier

```fsharp
val expandExpression : Expr -> Polynomial option
val combineLikeTerms : Term list -> Term list
val polynomialToString : Polynomial -> string

// Expands: (x + 1)(x - 1) → x² - 1
// Simplifies: x + x + 2x → 4x
```

### AlgebraSolver

```fsharp
val solveEquation : Equation -> float list

// Linear solver: ax + b = 0 → x = -b/a
// Quadratic solver: ax² + bx + c = 0 → uses discriminant formula
```

### AlgebraCodegen

```fsharp
val generateCode : Expr -> string list
val generateEquationCode : Equation -> string list

// Generates virtual machine code with register allocation
// Tracks register pressure and reuses registers efficiently
```

## Pattern Matching Examples

### Simple Expression Matching

```fsharp
match expr with
| Num n -> n
| Var v -> 1.0  // assume variable = 1 for evaluation
| Binop("+", left, right) -> eval left + eval right
| Binop("*", left, right) -> eval left * eval right
| _ -> failwith "Unknown expression"
```

### Recursive Descent Parsing

```fsharp
let rec parseExpression tokens =
    let left, tokens = parseTerm tokens
    parseExpressionHelper left tokens

and parseExpressionHelper left tokens =
    match tokens.Current.Type with
    | Plus | Minus as op ->
        let right, tokens = parseTerm (advance tokens)
        let expr = Binop(opToString op, left, right)
        parseExpressionHelper expr tokens
    | _ -> (left, tokens)
```

## Functional Paradigms Used

### 1. Immutability

```fsharp
// All data is immutable - creates new values instead of modifying
let terms = [Term(1.0, "x", 1); Term(2.0, "x", 1)]
let simplified = combineLikeTerms terms
// 'simplified' is a new list, 'terms' is unchanged
```

### 2. Pure Functions

```fsharp
// Functions have no side effects (except printing)
let tokenize input : Token list = ...
let parse tokens : Expr = ...
let expand expr : Polynomial = ...

// Same input always produces same output
```

### 3. Recursion

```fsharp
let rec parseExpression state =
    let left, state = parseTerm state
    parseExpressionHelper left state

and parseExpressionHelper left state =
    // recursion terminates when no more tokens to parse
    match state.Current.Type with
    | Plus | Minus -> parseExpressionHelper (newExpr) (nextState)
    | _ -> (left, state)
```

### 4. Discriminated Unions

```fsharp
type Expr =
    | Num of float           // Tagged union case
    | Var of string
    | Binop of string * Expr * Expr
    // Pattern matching ensures all cases handled
```

### 5. Option Type for Null Safety

```fsharp
val parseExpressionString : string -> Expr option
// Returns Some(expr) on success, None on failure
// Forces explicit None handling

match parseExpressionString input with
| Some expr -> printf "Parsed: %A" expr
| None -> printf "Parse failed"
```

## Performance Characteristics

| Operation | Complexity | Notes |
|-----------|-----------|-------|
| Tokenization | O(n) | Linear scan through input |
| Parsing | O(n) | Recursive descent, n = token count |
| Simplification | O(n²) | Combining like terms via grouping |
| Solving | O(1) | Quadratic formula is constant-time |
| Code Generation | O(n) | Single pass through expression |

## Compilation Modes

### JIT (Default)

```bash
dotnet build
dotnet run
# 200ms startup, ~100MB memory, GC pauses
```

### AoT (Optimized)

```bash
dotnet publish -c Release -r win-x64 --self-contained /p:PublishAot=true
# 5ms startup, ~15MB memory, 0 GC pauses
```

See [AOT_COMPILATION_GUIDE.md](../AOT_COMPILATION_GUIDE.md) for details.

## Testing & Debugging

### Pattern Matching for Debugging

```fsharp
let debugExpr expr =
    match expr with
    | Num n -> printfn "Number: %f" n
    | Var v -> printfn "Variable: %s" v
    | Binop(op, l, r) -> printfn "Operation: %s" op
    | _ -> printfn "Other expression"
```

### Incremental Development

Each module can be tested independently:

```fsharp
// Test tokenizer
let tokens = tokenize "2x + 3"
assert (List.length tokens = 5)

// Test parser
let expr = parseExpressionString "2x + 3"
assert (expr.IsSome)

// Test simplifier
let poly = expandExpression expr.Value
assert (combineLikeTerms poly |> List.length = 2)
```

## References

- [F# Official Documentation](https://learn.microsoft.com/en-us/dotnet/fsharp/)
- [F# Language Reference](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/)
- [Discriminated Unions](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/discriminated-unions)
- [Pattern Matching](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/pattern-matching)
