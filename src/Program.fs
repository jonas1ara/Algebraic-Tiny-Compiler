open AlgebraTypes
open AlgebraTokenizer
open AlgebraParser
open AlgebraSimplifier
open AlgebraSolver
open AlgebraCodegen

let printPolyResult (poly: Polynomial) =
    printfn "Simplified: %s" (polynomialToString poly)

let printSolutions (solutions: float list) =
    match solutions with
    | [] -> printfn "No solutions found"
    | solutions ->
        printfn "Solutions:"
        solutions |> List.iteri (fun i sol -> printfn "  x%d = %f" (i + 1) sol)

let processInput (input: string) =
    printfn "\n=== ALGEBRAIC TINY COMPILER ==="
    printfn "Input: %s" input
    printfn ""
    
    // Check if it's an algebraic equation
    match AlgebraParser.parseEquation input with
    | Some equation ->
        printfn "[EQUATION DETECTED]"
        
        // Simplify sides
        let leftSimplified = AlgebraSimplifier.expandExpression equation.Left
        let rightSimplified = AlgebraSimplifier.expandExpression equation.Right
        
        (match leftSimplified, rightSimplified with
        | Some lpoly, Some rpoly ->
            printfn "Left side:  %s" (polynomialToString lpoly)
            printfn "Right side: %s" (polynomialToString rpoly)
            
            // Solve the equation
            let solutions = AlgebraSolver.solveEquation equation
            printfn ""
            printSolutions solutions
            
            // Generate code
            printfn "\nGenerated Code:"
            let code = AlgebraCodegen.generateEquationCode equation
            code |> List.iter (printfn "%s")
        | _ -> printfn "Error parsing equation sides")
    
    | None ->
        // Try as a simple algebraic expression
        match AlgebraParser.parseExpressionString input with
        | Some expr ->
            printfn "[EXPRESSION DETECTED]"
            
            match AlgebraSimplifier.expandExpression expr with
            | Some poly ->
                printfn "Expanded: %s" (polynomialToString poly)
                printfn ""
                
                // Generate code
                printfn "Generated Code:"
                let code = AlgebraCodegen.generateCode expr
                code |> List.iter (fun line -> printfn "  %s" line)
            | None -> printfn "Error expanding expression"
        | None ->
            printfn "Error: Could not parse input"

[<EntryPoint>]
let main argv =
    printfn "╔═══════════════════════════════════════════════╗"
    printfn "║  ALGEBRAIC TINY COMPILER v1.0 - Interactive  ║"
    printfn "║  Understanding Compilers Through Algebra     ║"
    printfn "╚═══════════════════════════════════════════════╝"
    
    let mutable running = true
    while running do
        printfn "\n┌─ EXAMPLES ──────────────────────────────────┐"
        printfn "│  Expressions:                              │"
        printfn "│    • 2x + 5                                │"
        printfn "│    • x^2 - 3x + 2                         │"
        printfn "│  Equations:                                │"
        printfn "│    • 2x + 3 = 5                           │"
        printfn "│    • 2x^2 + 3x - 5 = 0                   │"
        printfn "└──────────────────────────────────────────┘"
        printfn "Type 'quit' to exit\n"
        
        printf "Enter expression: "
        let input = System.Console.ReadLine()
        
        if input.ToLower() = "quit" then
            running <- false
        else if not (System.String.IsNullOrWhiteSpace(input)) then
            processInput input
    
    printfn "\nGoodbye!"
    0
