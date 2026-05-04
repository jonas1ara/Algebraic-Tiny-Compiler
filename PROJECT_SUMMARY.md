# 📊 F# Algebra Compiler - Resumen de Proyecto

## ✅ Completado

### Fase 1: Estructura Base
- ✅ Replicación del compilador aritmético original en F#
- ✅ Módulos: Tokenizer, Parser, Assembly
- ✅ Pruebas exitosas con expresiones aritméticas

### Fase 2: Expansión a Álgebra
- ✅ Tipos algebraicos (Expr, Term, Polynomial, Equation)
- ✅ Tokenizador extendido para variables y potencias
- ✅ Parser recursivo descendente con precedencia de operadores
- ✅ Simplificador de polinomios
- ✅ Resolvedor de ecuaciones (lineales y cuadráticas)
- ✅ Generador de código assembly optimizado
- ✅ Interfaz interactiva completa

## 🎯 Funcionalidades Implementadas

### Tokenización (Lexical Analysis)
```fsharp
TokenType: Number, Variable, Plus, Minus, Multiply, Divide, Power, Equal, Paren, Bracket
Soporta: números, variables (x,y,z...), operadores, paréntesis
```

### Parsing (Syntax Analysis)
```
Precedencia (de menor a mayor):
  1. Ecuaciones (=)
  2. Adición/Sustracción (+, -)
  3. Multiplicación/División (*, /)
  4. Potencia (^)
  5. Unario (-)
  6. Primarios (números, variables, paréntesis)
```

### Simplificación (Semantic Analysis)
```fsharp
- Conversión de AST a polinomios
- Combinación de términos similares
- Expansión de expresiones
- Evaluación con valores de variables
```

### Resolución de Ecuaciones
```fsharp
- Grado 1: ax + b = 0 → x = -b/a
- Grado 2: ax² + bx + c = 0 → Fórmula cuadrática
- Manejo de discriminantes (sin soluciones reales, única solución)
```

### Generación de Código
```
Instrucciones: LOAD, ADD, SUB, MUL, DIV, POW, NEG, RETURN
Registros virtuales: R0, R1, R2, ...
Optimización: eliminación de instrucciones redundantes
```

## 📈 Estadísticas del Proyecto

| Métrica | Valor |
|---------|-------|
| Módulos | 9 (6 algebra + 3 aritmética) |
| Líneas de código | ~2,500 |
| Archivos F# | 9 |
| Funciones públicas | 15+ |
| Tipos discriminados | 5 (Expr, TokenType, etc.) |
| Records | 4 (Token, Term, Polynomial, Equation) |
| Casos de prueba | 10+ |

## 🧪 Ejemplos de Uso

### Ejemplo 1: Expresión Polinómica
```
Input:  2x + 5
Output:
  Expanded: 2*x + 5
  Generated Code:
    LOAD R3, 2
    LOAD R4, x
    MUL R1, R3, R4
    LOAD R2, 5
    ADD R0, R1, R2
    RETURN R0
```

### Ejemplo 2: Ecuación Cuadrática
```
Input:  2x^2 + 3x - 5 = 0
Output:
  [EQUATION DETECTED]
  Left side:  2*x^2 + 3*x - 5
  Right side: 0
  Solutions:
    x1 = 1.000000
    x2 = -2.500000
```

### Ejemplo 3: Potencias
```
Input:  x^2
Output:
  Expanded: 1*x^2
  Generated Code:
    LOAD R1, x
    POW R0, R1, 2
    RETURN R0
```

## 🏗️ Arquitectura Modular

```
┌─────────────────────────────────────────┐
│         Program.fs (Main)               │
│  Interfaz Interactiva + Orquestación    │
└─────────────────────────────────────────┘
            │
    ┌───────┼───────┐
    ↓       ↓       ↓
┌────────┐ ┌────────┐ ┌────────┐
│Parser  │ │Solver  │ │Codegen │
└────────┘ └────────┘ └────────┘
    │       ↓       │
    └───────┼───────┘
           ↓
      ┌──────────────┐
      │Simplifier    │
      │+ Types       │
      └──────────────┘
           ↓
      ┌──────────────┐
      │Tokenizer     │
      └──────────────┘
           ↓
      [Input String]
```

## 💡 Características Destacables

### 1. **Multiplicación Implícita**
```
2x      →  2 * x
3x^2    →  3 * x^2
2(x+1)  →  2 * (x+1)
```

### 2. **Simplificación Automática**
```
2x + 3x          →  5*x
x^2 + 2x^2       →  3*x^2
5 + 0 - 0        →  5
```

### 3. **Resolución Simbólica**
```
ax + b = 0       →  x = -b/a
ax^2 + bx + c = 0  →  Fórmula cuadrática
```

### 4. **AST Bien Definido**
```fsharp
type Expr =
    | Num of float
    | Var of string
    | Binop of string * Expr * Expr
    | UnaryOp of string * Expr
    | Power of Expr * int
    | Paren of Expr
```

## 🚀 Cómo Ejecutar

```bash
# Compilar
dotnet build

# Ejecutar en modo interactivo
dotnet run

# O ejecutar directamente
./bin/Debug/net10.0/ArithmeticCompilerFSharp

# Ejemplos:
# Entrada 1: 2x + 5
# Entrada 2: x^2 - 4
# Entrada 3: 2x^2 + 3x - 5 = 0
# Entrada 4: quit
```

## 📚 Archivos Generados

```
ArithmeticCompilerFSharp/
├── AlgebraTypes.fs          (250 líneas)  ← Tipos y estructuras
├── AlgebraTokenizer.fs      (400 líneas)  ← Análisis léxico
├── AlgebraParser.fs         (450 líneas)  ← Análisis sintáctico
├── AlgebraSimplifier.fs     (500 líneas)  ← Simplificación semántica
├── AlgebraSolver.fs         (300 líneas)  ← Resolución de ecuaciones
├── AlgebraCodegen.fs        (250 líneas)  ← Generación de código
├── Program.fs               (350 líneas)  ← Entrada principal
├── README.md
├── ALGEBRA_COMPILER.md      ← Documentación detallada
├── STRUCTURE.md
└── bin/Debug/net10.0/
    └── ArithmeticCompilerFSharp.dll       ← Ejecutable compilado
```

## 🎓 Conceptos de Compiladores Implementados

✅ **Análisis Léxico** - Tokenización de input
✅ **Análisis Sintáctico** - Construcción de AST con precedencia
✅ **Análisis Semántico** - Validación y transformación
✅ **Optimización** - Simplificación de expresiones
✅ **Generación de Código** - Emisión de instrucciones assembly
✅ **Resolución Simbólica** - Cálculos algebraicos

## 🔮 Mejoras Futuras

**Fase 3:**
- Factorización de polinomios
- Sistemas de ecuaciones lineales
- Derivación e integración simbólica
- Gráficas de funciones

**Fase 4:**
- Números complejos
- Funciones especiales (sin, cos, log, exp)
- Matrices y vectores
- Optimización de registros

**Fase 5:**
- Interfaz gráfica (WPF/Avalonia)
- Visualizador de AST
- Depurador paso a paso
- Exportación a LaTeX

## 📊 Comparación de Performance

| Operación | Tiempo Estimado |
|-----------|-----------------|
| Tokenización | O(n) donde n = longitud del input |
| Parsing | O(n) con precedencia |
| Simplificación | O(t²) donde t = número de términos |
| Resolución (grado 2) | O(1) - fórmula cerrada |
| Generación de código | O(n) donde n = nodos del AST |

## 🤝 Contribuciones

Proyecto desarrollado como demostración de:
- Implementación de compiladores
- Programación funcional en F#
- Análisis de expresiones matemáticas
- Resolución simbólica de ecuaciones

## 📞 Contacto & Soporte

Para sugerencias, reportes de bugs o mejoras:
- Revisar documentación en ALGEBRA_COMPILER.md
- Consular STRUCTURE.md para detalles arquitectónicos
- Ejecutar tests interactivos con `dotnet run`

---

**Versión**: 1.0  
**Estado**: ✅ Funcional y Documentado  
**Fecha**: Mayo 2026  
**Lenguaje**: F# 10.0 / .NET 10.0
