module Assembly

open Tokenizer

let generateAssembly (tokens: Token list) : unit =
    let rec generateHelper (tokens: Token list) (isFirst: bool) : unit =
        match tokens with
        | { Type = End } :: _ | [] -> ()
        | { Type = Number; Value = num } :: rest when isFirst ->
            printfn "LOAD %d" num
            generateHelper rest false
        | { Type = Plus } :: { Type = Number; Value = num } :: rest ->
            printfn "ADD %d" num
            generateHelper rest false
        | { Type = Minus } :: { Type = Number; Value = num } :: rest ->
            printfn "SUB %d" num
            generateHelper rest false
        | { Type = Multiplication } :: { Type = Number; Value = num } :: rest ->
            printfn "MUL %d" num
            generateHelper rest false
        | { Type = Division } :: { Type = Number; Value = num } :: rest ->
            printfn "DIV %d" num
            generateHelper rest false
        | _ :: rest -> generateHelper rest false

    generateHelper tokens true
