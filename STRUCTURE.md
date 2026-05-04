# Comparación de Estructura: Original (C) vs Réplica (F#)

## Estructura de Directorios

### Original (C)
```
arithmetic-compiler/
├── include/
│   ├── tokenizer.h
│   ├── parser.h
│   └── assembly.h
├── src/
│   ├── main.c
│   ├── tokenizer.c
│   ├── parser.c
│   └── assembly.c
├── CMakeLists.txt
└── README.md
```

### Réplica (F#)
```
ArithmeticCompilerFSharp/
├── Tokenizer.fs
├── Parser.fs
├── Assembly.fs
├── Program.fs
├── ArithmeticCompilerFSharp.fsproj
└── README.md
```

## Mapeo de Módulos

| C (Header + Source) | F# Module | Propósito |
|-------------------|-----------|-----------|
| tokenizer.h / tokenizer.c | Tokenizer.fs | Análisis léxico |
| parser.h / parser.c | Parser.fs | Análisis sintáctico |
| assembly.h / assembly.c | Assembly.fs | Generación de código |
| main.c | Program.fs | Punto de entrada |

## Equivalencia de Tipos

### Tipos de Tokens

**C:**
```c
typedef enum { 
    TOKEN_NUMBER, 
    TOKEN_PLUS, 
    TOKEN_MINUS, 
    TOKEN_MULTIPLICATION, 
    TOKEN_DIVISION, 
    TOKEN_END 
} TokenType;

typedef struct {
    TokenType type;
    int value;
} Token;
```

**F#:**
```fsharp
type TokenType = 
    | Number
    | Plus
    | Minus
    | Multiplication
    | Division
    | End

type Token = {
    Type: TokenType
    Value: int
}
```

## Flujo de Compilación

Ambas versiones siguen el mismo flujo:

```
Input String
    ↓
Tokenizer (análisis léxico)
    ↓
Token List
    ↓
Parser (análisis sintáctico + evaluación)
    ↓
Resultado numérico
    ↓
Assembly Generator (generación de código)
    ↓
Assembly Code Output
```

## Ejemplo de Ejecución

### Entrada
```
3 + 4 - 2
```

### Tokens
```
[Number(3), Plus, Number(4), Minus, Number(2), End]
```

### Resultado de Parsing
```
5
```

### Código Assembly Generado
```
LOAD 3
ADD 4
SUB 2
```

## Diferencias Paradigmáticas

### C (Imperativo)
- Gestión manual de memoria
- Estado mutable (índices, buffers)
- Funciones que modifican parámetros
- Manejo explícito de errores con códigos de retorno

### F# (Funcional)
- Gestión automática de memoria (GC)
- Datos inmutables
- Recursión en lugar de bucles
- Pattern matching para control de flujo
- Tipos discriminados para representar alternativas

## Rendimiento y Características

| Característica | C | F# |
|----------------|---|-----|
| Velocidad | ⚡⚡⚡ Muy rápido | ⚡⚡ Rápido |
| Memoria | Manual (eficiente) | Automática (GC) |
| Seguridad | Manual | Más seguro (tipos) |
| Legibilidad | Media | Alta |
| Mantenibilidad | Media | Alta |
| Concisión | Media | Alta |

## Conclusión

La réplica en F# mantiene la estructura lógica del compilador original mientras aprovecha los beneficios del paradigma funcional:

✅ **Ventajas:**
- Código más conciso y expresivo
- Mejor seguridad de tipos
- Pattern matching potente
- Pruebas más fáciles con funciones puras

📌 **Consideraciones:**
- Mayor uso de memoria en tiempo de ejecución
- Requiere pensamiento recursivo
- Mejor para expresar transformaciones de datos
