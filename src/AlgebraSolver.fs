module AlgebraSolver

open AlgebraTypes
open AlgebraSimplifier

let private simplifyTerms (terms: Term list) : Term list =
    terms
    |> List.groupBy (fun t -> (t.Variable, t.Power))
    |> List.map (fun ((_, _), group) ->
        let sumCoeff = group |> List.sumBy (fun t -> t.Coefficient)
        { group.[0] with Coefficient = sumCoeff }
    )
    |> List.filter (fun t -> t.Coefficient <> 0.0)
    |> List.sortByDescending (fun t -> (t.Power, t.Variable))

let exprToString (expr: Expr) : string =
    let rec exprToStringRec (expr: Expr) : string =
        match expr with
        | Num n -> n.ToString()
        | Var v -> v
        | Binop(op, l, r) -> "(" + exprToStringRec l + " " + op + " " + exprToStringRec r + ")"
        | Power(e, p) -> exprToStringRec e + "^" + p.ToString()
        | Paren e -> "(" + exprToStringRec e + ")"
        | UnaryOp(op, e) -> op + exprToStringRec e
    exprToStringRec expr

/// Solves simple linear equations: ax + b = 0
let solveLinearEquation (poly: Polynomial) : float option =
    match poly.Terms with
    | [] -> None
    | terms when List.length terms <= 2 ->
        let constant = terms |> List.tryFind (fun t -> t.Power = 0)
        let linear = terms |> List.tryFind (fun t -> t.Power = 1)
        
        match (constant, linear) with
        | (Some c, Some l) -> Some (-(c.Coefficient) / l.Coefficient)
        | (None, Some l) -> Some 0.0
        | _ -> None
    | _ -> None

/// Solves quadratic equations: ax^2 + bx + c = 0 using the quadratic formula
let solveQuadraticEquation (poly: Polynomial) : float list =
    let a = poly.Terms |> List.tryFind (fun t -> t.Power = 2) |> Option.map (fun t -> t.Coefficient) |> Option.defaultValue 0.0
    let b = poly.Terms |> List.tryFind (fun t -> t.Power = 1) |> Option.map (fun t -> t.Coefficient) |> Option.defaultValue 0.0
    let c = poly.Terms |> List.tryFind (fun t -> t.Power = 0) |> Option.map (fun t -> t.Coefficient) |> Option.defaultValue 0.0
    
    if a = 0.0 then []
    else
        let discriminant = b * b - 4.0 * a * c
        if discriminant < 0.0 then
            []
        elif discriminant = 0.0 then
            [-(b) / (2.0 * a)]
        else
            let sqrtDisc = sqrt discriminant
            [
                (-(b) + sqrtDisc) / (2.0 * a);
                (-(b) - sqrtDisc) / (2.0 * a)
            ]

/// Solves a general equation
let solveEquation (equation: Equation) : float list =
    // Moves everything to the left side: Left - Right = 0
    let leftPoly = exprToPolynomial equation.Left |> Option.defaultValue { Terms = [] }
    let rightPoly = exprToPolynomial equation.Right |> Option.defaultValue { Terms = [] }
    
    let negatedRight = rightPoly.Terms |> List.map (fun t -> { t with Coefficient = -t.Coefficient })
    let combined = (leftPoly.Terms @ negatedRight) |> simplifyTerms
    let resultPoly = { Terms = combined }
    
    let maxPower =
        if resultPoly.Terms.IsEmpty then 0
        else resultPoly.Terms |> List.maxBy (fun t -> t.Power) |> fun t -> t.Power
    
    match maxPower with
    | 0 -> [] // Constant equation
    | 1 ->
        match solveLinearEquation resultPoly with
        | Some x -> [x]
        | None -> []
    | 2 -> solveQuadraticEquation resultPoly
    | _ -> [] // We don't support higher degrees yet

/// Generates simplification steps
let simplificationSteps (expr: Expr) : string list =
    [
        "Original expression: " + exprToString expr
        "Analyzing structure..."
    ]
