module Parser

open Tokenizer

let parse (tokens: Token list) : int option =
    let rec parseHelper (tokens: Token list) (result: int) : int option =
        match tokens with
        | { Type = End } :: _ -> Some result
        | { Type = Plus } :: { Type = Number; Value = num } :: rest ->
            parseHelper rest (result + num)
        | { Type = Minus } :: { Type = Number; Value = num } :: rest ->
            parseHelper rest (result - num)
        | { Type = Multiplication } :: { Type = Number; Value = num } :: rest ->
            parseHelper rest (result * num)
        | { Type = Division } :: { Type = Number; Value = num } :: rest ->
            if num = 0 then None
            else parseHelper rest (result / num)
        | _ -> None

    match tokens with
    | { Type = Number; Value = firstNum } :: rest ->
        parseHelper rest firstNum
    | _ -> None
