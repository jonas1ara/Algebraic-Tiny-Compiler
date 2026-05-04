module AlgebraTokenizer

type TokenType = 
    | Number
    | Variable
    | Plus
    | Minus
    | Multiply
    | Divide
    | Power
    | Equal
    | LeftParen
    | RightParen
    | LeftBracket
    | RightBracket
    | Comma
    | End

type Token = {
    Type: TokenType
    Value: string
}

let tokenize (input: string) : Token list =
    let rec tokenizeHelper (chars: char list) (tokens: Token list) =
        match chars with
        | [] -> tokens @ [{ Type = End; Value = "" }]
        
        // Numbers
        | c :: rest when System.Char.IsDigit(c) ->
            let numStr = System.String.Concat(List.takeWhile (fun ch -> System.Char.IsDigit(ch) || ch = '.') (c :: rest))
            let remainingChars = List.skip numStr.Length (c :: rest)
            tokenizeHelper remainingChars (tokens @ [{ Type = Number; Value = numStr }])
        
        // Variables (letters a-z, A-Z)
        | c :: rest when System.Char.IsLetter(c) ->
            let varStr = System.String.Concat(List.takeWhile System.Char.IsLetter (c :: rest))
            let remainingChars = List.skip varStr.Length (c :: rest)
            tokenizeHelper remainingChars (tokens @ [{ Type = Variable; Value = varStr }])
        
        // Operators
        | '+' :: rest -> tokenizeHelper rest (tokens @ [{ Type = Plus; Value = "+" }])
        | '-' :: rest -> tokenizeHelper rest (tokens @ [{ Type = Minus; Value = "-" }])
        | '*' :: rest -> tokenizeHelper rest (tokens @ [{ Type = Multiply; Value = "*" }])
        | '/' :: rest -> tokenizeHelper rest (tokens @ [{ Type = Divide; Value = "/" }])
        | '^' :: rest -> tokenizeHelper rest (tokens @ [{ Type = Power; Value = "^" }])
        | '=' :: rest -> tokenizeHelper rest (tokens @ [{ Type = Equal; Value = "=" }])
        
        // Parentheses and brackets
        | '(' :: rest -> tokenizeHelper rest (tokens @ [{ Type = LeftParen; Value = "(" }])
        | ')' :: rest -> tokenizeHelper rest (tokens @ [{ Type = RightParen; Value = ")" }])
        | '[' :: rest -> tokenizeHelper rest (tokens @ [{ Type = LeftBracket; Value = "[" }])
        | ']' :: rest -> tokenizeHelper rest (tokens @ [{ Type = RightBracket; Value = "]" }])
        | ',' :: rest -> tokenizeHelper rest (tokens @ [{ Type = Comma; Value = "," }])
        
        // Whitespace
        | c :: rest when System.Char.IsWhiteSpace(c) -> tokenizeHelper rest tokens
        
        // Unknown characters
        | _ :: rest -> tokenizeHelper rest tokens

    tokenizeHelper (input.ToCharArray() |> Array.toList) []
