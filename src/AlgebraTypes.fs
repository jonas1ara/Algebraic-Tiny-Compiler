module AlgebraTypes

/// Expresión algebraica abstracta
type Expr =
    | Num of float
    | Var of string
    | Binop of string * Expr * Expr  // op, left, right
    | UnaryOp of string * Expr       // op, expr
    | Power of Expr * int            // base, exponent
    | Paren of Expr

/// Término en un polinomio (coeficiente * variable^potencia)
type Term = {
    Coefficient: float
    Variable: string
    Power: int
}

/// Polinomio (suma de términos)
type Polynomial = {
    Terms: Term list
}

/// Ecuación (izquierda = derecha)
type Equation = {
    Left: Expr
    Right: Expr
}

/// Matriz
type Matrix = {
    Rows: int
    Cols: int
    Data: float[,]
}

/// AST simplificado después del análisis
type SimplifiedExpr =
    | NumVal of float
    | VarExpr of string
    | PolyExpr of Polynomial
    | EquationExpr of Equation
    | MatrixExpr of Matrix
    | ErrorExpr of string
