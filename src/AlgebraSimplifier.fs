module AlgebraSimplifier

open AlgebraTypes

/// Converts an expression to a polynomial
let rec exprToPolynomial (expr: Expr) : Polynomial option =
    match expr with
    | Num n ->
        if n = 0.0 then Some { Terms = [] }
        else Some { Terms = [{ Coefficient = n; Variable = ""; Power = 0 }] }
    
    | Var v ->
        Some { Terms = [{ Coefficient = 1.0; Variable = v; Power = 1 }] }
    
    | Power(Var v, p) ->
        Some { Terms = [{ Coefficient = 1.0; Variable = v; Power = p }] }
    
    | Binop("+", left, right) ->
        match (exprToPolynomial left, exprToPolynomial right) with
        | (Some l, Some r) -> 
            let combined = l.Terms @ r.Terms |> simplifyTerms
            Some { Terms = combined }
        | _ -> None
    
    | Binop("-", left, right) ->
        match (exprToPolynomial left, exprToPolynomial right) with
        | (Some l, Some r) ->
            let negatedRight = r.Terms |> List.map (fun t -> { t with Coefficient = -t.Coefficient })
            let combined = l.Terms @ negatedRight |> simplifyTerms
            Some { Terms = combined }
        | _ -> None
    
    | Binop("*", left, right) ->
        match (exprToPolynomial left, exprToPolynomial right) with
        | (Some l, Some r) ->
            let products =
                l.Terms
                |> List.collect (fun t1 ->
                    r.Terms 
                    |> List.map (fun t2 ->
                        {
                            Coefficient = t1.Coefficient * t2.Coefficient
                            Variable = if t1.Variable = "" then t2.Variable elif t2.Variable = "" then t1.Variable elif t1.Variable = t2.Variable then t1.Variable else ""
                            Power = t1.Power + t2.Power
                        }
                    )
                )
            Some { Terms = products |> simplifyTerms }
        | _ -> None
    
    | Paren e -> exprToPolynomial e
    | _ -> None

and private simplifyTerms (terms: Term list) : Term list =
    terms
    |> List.groupBy (fun t -> (t.Variable, t.Power))
    |> List.map (fun ((_, _), group) ->
        let sumCoeff = group |> List.sumBy (fun t -> t.Coefficient)
        { group.[0] with Coefficient = sumCoeff }
    )
    |> List.filter (fun t -> t.Coefficient <> 0.0)
    |> List.sortByDescending (fun t -> (t.Power, t.Variable))

/// Expands an expression (multiplies all terms)
let expandExpression (expr: Expr) : Polynomial option =
    exprToPolynomial expr

/// Simplifies an expression
let simplifyExpression (expr: Expr) : Expr =
    match exprToPolynomial expr with
    | Some poly ->
        match poly.Terms with
        | [] -> Num 0.0
        | [single] when single.Variable = "" && single.Power = 0 ->
            Num single.Coefficient
        | [single] when single.Variable <> "" && single.Coefficient = 1.0 && single.Power = 1 ->
            Var single.Variable
        | _ -> expr
    | None -> expr

/// Evaluates an expression with values for variables
let rec evaluateExpr (expr: Expr) (vars: Map<string, float>) : float option =
    match expr with
    | Num n -> Some n
    | Var v ->
        match Map.tryFind v vars with
        | Some value -> Some value
        | None -> None
    | Binop(op, left, right) ->
        match (evaluateExpr left vars, evaluateExpr right vars) with
        | (Some l, Some r) ->
            match op with
            | "+" -> Some (l + r)
            | "-" -> Some (l - r)
            | "*" -> Some (l * r)
            | "/" -> if r <> 0.0 then Some (l / r) else None
            | _ -> None
        | _ -> None
    | Power(base', p) ->
        match evaluateExpr base' vars with
        | Some b -> Some (b ** float p)
        | None -> None
    | UnaryOp(op, expr) ->
        match evaluateExpr expr vars with
        | Some v when op = "-" -> Some (-v)
        | Some v -> Some v
        | None -> None
    | Paren e -> evaluateExpr e vars

/// Converts a polynomial to a readable string
let polynomialToString (poly: Polynomial) : string =
    match poly.Terms with
    | [] -> "0"
    | terms ->
        terms
        |> List.mapi (fun i term ->
            let sign = if i = 0 then "" else if term.Coefficient > 0.0 then " + " else " - "
            let coeff = abs term.Coefficient
            let coeffStr =
                if term.Variable = "" || term.Power = 0 then
                    if coeff = 1.0 && i > 0 then "" else coeff.ToString()
                else
                    if coeff = 1.0 && i > 0 then "" else (coeff.ToString() + "*")
            let varStr = 
                if term.Variable = "" then ""
                elif term.Power = 0 then "" 
                elif term.Power = 1 then term.Variable 
                else term.Variable + "^" + term.Power.ToString()
            sign + coeffStr + varStr
        )
        |> String.concat ""
