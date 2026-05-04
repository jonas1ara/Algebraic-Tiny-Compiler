# F# Algebra Compiler - Documentación Completa

## 📋 Descripción General

**ArithmeticCompilerFSharp** es un compilador algebraico completo escrito en F# que expande el compilador aritmético original con capacidades avanzadas de álgebra. Soporta:

✅ **Expresiones Aritméticas** - Operaciones básicas: +, -, *, /
✅ **Variables** - Soporte para variables: x, y, z, etc.
✅ **Polinomios** - Expansión y simplificación de polinomios
✅ **Ecuaciones** - Resolución de ecuaciones lineales y cuadráticas  
✅ **Potencias** - Soporte de exponentes: x^2, x^3, etc.
✅ **Multiplicación Implícita** - Reconoce 2x como 2*x

## 🏗️ Arquitectura

### Módulos Principales

```
┌─────────────────────────────────────────────────────────────┐
│                   Program.fs (Entry Point)                  │
└─────────────────────────────────────────────────────────────┘
                            ↓
  ┌────────────────┬────────────────┬────────────────────┐
  ↓                ↓                ↓                    ↓
AlgebraParser   AlgebraSimplifier AlgebraSolver   AlgebraCodegen
  (Parsing)      (Simplification)  (Solving)        (Code Gen)
  ↓                ↓                ↓                    ↓
  └────────────────┴────────────────┴────────────────────┘
                            ↓
              AlgebraTokenizer (Lexical Analysis)
                            ↓
                     AlgebraTypes (Type Definitions)
```

### Flujo de Datos

```
Input String
    ↓
AlgebraTokenizer (Lexical Analysis)
Token list: [Number(2), Multiply(?), Variable(x), Plus, Number(5)]
    ↓
AlgebraParser (Syntax Analysis)
AST: Binop("+", Binop("*", Num(2.0), Var("x")), Num(5.0))
    ↓
AlgebraSimplifier (Semantic Analysis)
Polynomial: { Terms = [{Coefficient=2, Variable="x", Power=1}, {Coefficient=5, Variable="", Power=0}] }
    ↓
AlgebraCodegen (Code Generation)
Assembly Code: 
  LOAD R3, 2
  LOAD R4, x
  MUL R1, R3, R4
  LOAD R2, 5
  ADD R0, R1, R2
  RETURN R0
```

## 📦 Módulos Detallados

### 1. **AlgebraTypes.fs** - Definición de Tipos

Define las estructuras de datos fundamentales:

```fsharp
type Expr =
    | Num of float              // Número literal: 42
    | Var of string             // Variable: x, y, z
    | Binop of string * Expr * Expr  // Operador binario: a + b
    | UnaryOp of string * Expr  // Operador unario: -x
    | Power of Expr * int       // Potencia: x^2
    | Paren of Expr             // Paréntesis: (a + b)

type Term = {                   // Término de polinomio
    Coefficient: float          // 3 en 3x^2
    Variable: string            // x en 3x^2
    Power: int                  // 2 en 3x^2
}

type Polynomial = {             // Polinomio: 3x^2 + 2x - 5
    Terms: Term list
}

type Equation = {               // Ecuación: 2x + 5 = 0
    Left: Expr
    Right: Expr
}
```

### 2. **AlgebraTokenizer.fs** - Análisis Léxico

Convierte strings en tokens reconocibles:

```fsharp
type TokenType = 
    | Number      // 2, 3.5
    | Variable    // x, y, variable
    | Plus        // +
    | Minus       // -
    | Multiply    // *
    | Divide      // /
    | Power       // ^
    | Equal       // =
    | LeftParen   // (
    | RightParen  // )
    | End         // Final

// Ejemplos:
// "2x + 5"  → [Number(2), Variable(x), Plus, Number(5), End]
// "x^2 = 9" → [Variable(x), Power, Number(2), Equal, Number(9), End]
```

### 3. **AlgebraParser.fs** - Análisis Sintáctico

Construye un AST mediante parsing recursivo descendente:

**Precedencia de Operadores** (de menor a mayor):
1. Adición (+) y Sustracción (-)
2. Multiplicación (*) y División (/)
3. Potencia (^)
4. Unario (-)
5. Primarios (números, variables, paréntesis)

**Ejemplo de Parsing:**
```
Entrada: 2x^2 + 3x - 5 = 0

Ecuación:
  Left:  2x^2 + 3x - 5
         = Binop("+", Binop("+", Binop("*", Num(2), Power(Var(x), 2)), Binop("*", Num(3), Var(x))), Num(-5))
  Right: 0
         = Num(0)
```

### 4. **AlgebraSimplifier.fs** - Análisis Semántico

Convierte expresiones en polinomios simplificados:

**Funciones Principales:**
- `exprToPolynomial` - Convierte Expr → Polynomial
- `expandExpression` - Expande y simplifica
- `evaluateExpr` - Evalúa con valores de variables
- `polynomialToString` - Formatea polinomio como string

**Ejemplo:**
```
Input:  (2x + 3)(x - 1)
Output: { Terms = [{Coefficient=2, Variable=x, Power=2}, 
                   {Coefficient=1, Variable=x, Power=1}, 
                   {Coefficient=-3, Variable="", Power=0}] }
String: "2*x^2 + 1*x - 3"
```

### 5. **AlgebraSolver.fs** - Resolución de Ecuaciones

Resuelve ecuaciones moviendo todo al lado izquierdo:

**Métodos Soportados:**
- **Grado 0** - Ecuaciones constantes
- **Grado 1** - Ecuaciones lineales: ax + b = 0 → x = -b/a
- **Grado 2** - Ecuaciones cuadráticas: ax² + bx + c = 0
  - Fórmula cuadrática: x = (-b ± √(b² - 4ac)) / 2a

**Ejemplo:**
```
Input:  2x^2 + 3x - 5 = 0

Pasos:
  1. Mover a lado izquierdo: 2x^2 + 3x - 5
  2. Identificar coeficientes: a=2, b=3, c=-5
  3. Calcular discriminante: Δ = 9 - 4(2)(-5) = 49
  4. Aplicar fórmula: x = (-3 ± 7) / 4
  5. Soluciones: x₁ = 1, x₂ = -2.5

Output: [1.0; -2.5]
```

### 6. **AlgebraCodegen.fs** - Generación de Código

Emite instrucciones assembly-like optimizadas:

**Instrucciones Soportadas:**
```
LOAD reg, value     # Carga un valor en registro
ADD reg, reg1, reg2 # Suma
SUB reg, reg1, reg2 # Resta
MUL reg, reg1, reg2 # Multiplicación
DIV reg, reg1, reg2 # División
POW reg, reg1, exp  # Potencia
NEG reg, reg1       # Negación
RETURN reg          # Retorna resultado
```

**Ejemplo de Generación:**
```
Input:  2x + 5
Output:
  LOAD R3, 2      # Cargar 2
  LOAD R4, x      # Cargar variable x
  MUL R1, R3, R4  # Multiplicar: 2*x
  LOAD R2, 5      # Cargar 5
  ADD R0, R1, R2  # Sumar: (2*x) + 5
  RETURN R0       # Retornar resultado
```

## 🚀 Uso

### Compilar el Proyecto

```bash
cd ArithmeticCompilerFSharp
dotnet build
```

### Ejecutar el Compilador

```bash
dotnet run
```

### Ejemplos Interactivos

**Ejemplo 1: Expresión Simple**
```
Entrada: 2x + 5
Salida:
  [EXPRESSION DETECTED]
  Expanded: 2*x + 5
  
  Generated Code:
    LOAD R3, 2
    LOAD R4, x
    MUL R1, R3, R4
    LOAD R2, 5
    ADD R0, R1, R2
    RETURN R0
```

**Ejemplo 2: Polinomio**
```
Entrada: x^2 - 4
Salida:
  [EXPRESSION DETECTED]
  Expanded: 1*x^2 - 4
```

**Ejemplo 3: Ecuación Cuadrática**
```
Entrada: 2x^2 + 3x - 5 = 0
Salida:
  [EQUATION DETECTED]
  Left side:  2*x^2 + 3*x - 5
  Right side: 0
  
  Solutions:
    x1 = 1.000000
    x2 = -2.500000
```

## 📊 Comparativa: Original (C) vs Réplica (F#)

| Característica | Aritmética (C) | Álgebra (F#) |
|---|---|---|
| Tokenización | Números + Operadores | +Variables, +Potencias |
| Parsing | Simple (números) | Recursivo descendente |
| Evaluación | In-situ | AST → Polinomio |
| Simplificación | No | Combine like terms |
| Resolución | No | Lineal + Cuadrática |
| Generación | Assembly simple | Assembly optimizado |
| Complejidad | O(n) | O(n²) en polinomios |

## 🔧 Características Avanzadas

### Multiplicación Implícita
```
2x          → 2 * x  ✓
3x^2        → 3 * x^2  ✓
2(x+1)      → 2 * (x+1)  ✓
```

### Simplificación Automática
```
2x + 3x     → 5*x     ✓
x^2 + 2x^2  → 3*x^2   ✓
5 + 0       → 5       ✓
```

### Resolución Simbólica
```
ax + b = 0  → x = -b/a     ✓
ax^2 + bx + c = 0  → fórmula cuadrática  ✓
```

## 🐛 Limitaciones Actuales

- No soporta ecuaciones de grado > 2
- No maneja números complejos
- No soporta funciones (sin, cos, log, etc.)
- No hay soporte para matrices
- No hay factorización simbólica

## 📈 Mejoras Futuras

1. **Factorización** - Factorizar polinomios
2. **Sistemas de Ecuaciones** - Resolver sistemas lineales
3. **Funciones Especiales** - Trigonometría, logaritmos
4. **Matrices** - Operaciones matriciales
5. **Números Complejos** - Raíces complejas
6. **Optimización** - Mejor generación de código
7. **Gráficas** - Visualizar polinomios

## 📚 Estructura de Directorios

```
ArithmeticCompilerFSharp/
├── AlgebraTypes.fs              # Definiciones de tipos
├── AlgebraTokenizer.fs          # Análisis léxico
├── AlgebraParser.fs             # Análisis sintáctico
├── AlgebraSimplifier.fs         # Análisis semántico
├── AlgebraSolver.fs             # Resolución de ecuaciones
├── AlgebraCodegen.fs            # Generación de código
├── Program.fs                   # Entrada principal
├── ArithmeticCompilerFSharp.fsproj  # Configuración del proyecto
├── README.md                    # Este archivo
├── STRUCTURE.md                 # Comparativa de estructura
├── Tokenizer.fs                 # Tokenizer aritmético original
├── Parser.fs                    # Parser aritmético original
├── Assembly.fs                  # Assembly aritmético original
└── bin/                         # Binarios compilados
```

## 🎓 Conceptos de Compiladores Implementados

1. **Análisis Léxico** - Reconocer tokens del input
2. **Análisis Sintáctico** - Construir AST con precedencia de operadores
3. **Análisis Semántico** - Validar y transformar el AST
4. **Optimización** - Simplificar expresiones
5. **Generación de Código** - Emitir instrucciones optimizadas

## 📝 Licencia

Ver archivo LICENSE en el directorio raíz.

---

**Autor**: Copilot CLI  
**Lenguaje**: F# 10.0  
**Framework**: .NET 10.0  
**Año**: 2026
