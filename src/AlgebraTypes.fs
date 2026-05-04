module AlgebraTypes

/// Abstract algebraic expression
type Expr =
    | Num of float
    | Var of string
    | Binop of string * Expr * Expr  // operator, left, right
    | UnaryOp of string * Expr       // operator, expression
    | Power of Expr * int            // base, exponent
    | Paren of Expr

/// Term in a polynomial (coefficient * variable^power)
type Term = {
    Coefficient: float
    Variable: string
    Power: int
}

/// Polynomial (sum of terms)
type Polynomial = {
    Terms: Term list
}

/// Equation (left = right)
type Equation = {
    Left: Expr
    Right: Expr
}

/// Matrix
type Matrix = {
    Rows: int
    Cols: int
    Data: float[,]
}

/// Simplified AST after semantic analysis
type SimplifiedExpr =
    | NumVal of float
    | VarExpr of string
    | PolyExpr of Polynomial
    | EquationExpr of Equation
    | MatrixExpr of Matrix
    | ErrorExpr of string
