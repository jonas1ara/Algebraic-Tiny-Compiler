module AlgebraCodegen

open AlgebraTypes
open AlgebraSimplifier

let generateCode (expr: Expr) : string list =
    let instructions = System.Collections.Generic.List<string>()
    let mutable regCounter = 0
    
    let newReg () =
        let reg = "R" + regCounter.ToString()
        regCounter <- regCounter + 1
        reg
    
    let rec genExpr (expr: Expr) (targetReg: string) : unit =
        match expr with
        | Num n ->
            instructions.Add($"LOAD {targetReg}, {n}")
        
        | Var v ->
            instructions.Add($"LOAD {targetReg}, {v}")
        
        | Binop(op, left, right) ->
            let leftReg = newReg()
            let rightReg = newReg()
            genExpr left leftReg
            genExpr right rightReg
            
            let opCode = 
                match op with
                | "+" -> "ADD"
                | "-" -> "SUB"
                | "*" -> "MUL"
                | "/" -> "DIV"
                | _ -> "UNKNOWN"
            
            instructions.Add($"{opCode} {targetReg}, {leftReg}, {rightReg}")
        
        | Power(base', exp) ->
            let baseReg = newReg()
            genExpr base' baseReg
            instructions.Add($"POW {targetReg}, {baseReg}, {exp}")
        
        | UnaryOp(op, expr) ->
            let exprReg = newReg()
            genExpr expr exprReg
            match op with
            | "-" -> instructions.Add($"NEG {targetReg}, {exprReg}")
            | _ -> ()
        
        | Paren e ->
            genExpr e targetReg
    
    let resultReg = newReg()
    genExpr expr resultReg
    instructions.Add($"RETURN {resultReg}")
    
    instructions |> Seq.toList

let generateEquationCode (equation: Equation) : string list =
    let instructions = System.Collections.Generic.List<string>()
    
    instructions.Add("=== SOLVING EQUATION ===")
    instructions.Add("")
    instructions.Add("Left side code:")
    generateCode equation.Left |> List.iter (fun line -> instructions.Add("  " + line))
    
    instructions.Add("")
    instructions.Add("Right side code:")
    generateCode equation.Right |> List.iter (fun line -> instructions.Add("  " + line))
    
    instructions.Add("")
    instructions.Add("Setting Left = Right and solving...")
    
    instructions |> Seq.toList

let optimizeCode (code: string list) : string list =
    code
    |> List.filter (fun line -> not (line.StartsWith("LOAD R") && line.EndsWith("0")))
    |> List.distinctBy (fun line -> line)
