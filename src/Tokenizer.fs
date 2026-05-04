module Tokenizer

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

let tokenize (input: string) : Token list =
    let rec tokenizeHelper (chars: char list) (tokens: Token list) =
        match chars with
        | [] -> tokens @ [{ Type = End; Value = 0 }]
        | c :: rest when System.Char.IsDigit(c) ->
            let numStr = System.String.Concat(List.takeWhile System.Char.IsDigit (c :: rest))
            let num = int numStr
            let remainingChars = List.skip numStr.Length (c :: rest)
            tokenizeHelper remainingChars (tokens @ [{ Type = Number; Value = num }])
        | '+' :: rest ->
            tokenizeHelper rest (tokens @ [{ Type = Plus; Value = 0 }])
        | '-' :: rest ->
            tokenizeHelper rest (tokens @ [{ Type = Minus; Value = 0 }])
        | '*' :: rest ->
            tokenizeHelper rest (tokens @ [{ Type = Multiplication; Value = 0 }])
        | '/' :: rest ->
            tokenizeHelper rest (tokens @ [{ Type = Division; Value = 0 }])
        | c :: rest when System.Char.IsWhiteSpace(c) ->
            tokenizeHelper rest tokens
        | _ :: rest ->
            tokenizeHelper rest tokens

    tokenizeHelper (input.ToCharArray() |> Array.toList) []
