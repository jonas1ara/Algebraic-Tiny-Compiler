module AlgebraParser

open AlgebraTypes
open AlgebraTokenizer

let private matches (token: Token) (types: TokenType list) : bool =
    List.contains token.Type types

type ParserState = {
    Tokens: Token list
    Position: int
}

let private currentToken (state: ParserState) : Token =
    if state.Position < state.Tokens.Length then
        state.Tokens.[state.Position]
    else
        { Type = End; Value = "" }

let private advance (state: ParserState) : ParserState =
    { state with Position = state.Position + 1 }

let rec private parseExpression (state: ParserState) : Expr * ParserState =
    let (left, state) = parseTerm state
    
    let rec parseAddSub (expr: Expr) (state: ParserState) : Expr * ParserState =
        match currentToken state with
        | { Type = Plus } ->
            let state = advance state
            let (right, state) = parseTerm state
            parseAddSub (Binop("+", expr, right)) state
        | { Type = Minus } ->
            let state = advance state
            let (right, state) = parseTerm state
            parseAddSub (Binop("-", expr, right)) state
        | _ -> (expr, state)
    
    parseAddSub left state

and private parseTerm (state: ParserState) : Expr * ParserState =
    let (left, state) = parseFactor state
    
    let rec parseMulDiv (expr: Expr) (state: ParserState) : Expr * ParserState =
        match currentToken state with
        | { Type = Multiply } ->
            let state = advance state
            let (right, state) = parseFactor state
            parseMulDiv (Binop("*", expr, right)) state
        | { Type = Divide } ->
            let state = advance state
            let (right, state) = parseFactor state
            parseMulDiv (Binop("/", expr, right)) state
        // Implicit multiplication: Number followed by Variable, or ) followed by (
        | { Type = Variable } when not (matches (currentToken state) [TokenType.Plus; TokenType.Minus; TokenType.End]) ->
            let (right, state) = parseFactor state
            parseMulDiv (Binop("*", expr, right)) state
        | { Type = LeftParen } ->
            let (right, state) = parseFactor state
            parseMulDiv (Binop("*", expr, right)) state
        | _ -> (expr, state)
    
    parseMulDiv left state

and private parseFactor (state: ParserState) : Expr * ParserState =
    let (base', state) = parsePower state
    (base', state)

and private parsePower (state: ParserState) : Expr * ParserState =
    let (left, state) = parseUnary state
    
    match currentToken state with
    | { Type = Power } ->
        let state = advance state
        let token = currentToken state
        (match token.Type with
        | Number ->
            let exp = int (float token.Value)
            let state = advance state
            (Expr.Power(left, exp), state)
        | _ -> (left, state))
    | _ -> (left, state)

and private parseUnary (state: ParserState) : Expr * ParserState =
    match currentToken state with
    | { Type = Minus } ->
        let state = advance state
        let (expr, state) = parsePrimary state
        (UnaryOp("-", expr), state)
    | _ -> parsePrimary state

and private parsePrimary (state: ParserState) : Expr * ParserState =
    match currentToken state with
    | { Type = Number } ->
        let value = float (currentToken state).Value
        (Num value, advance state)
    | { Type = Variable } ->
        let varName = (currentToken state).Value
        (Var varName, advance state)
    | { Type = LeftParen } ->
        let state = advance state
        let (expr, state) = parseExpression state
        match currentToken state with
        | { Type = RightParen } -> (Paren expr, advance state)
        | _ -> (expr, state)
    | _ -> (Num 0.0, state)

let parseEquation (input: string) : Equation option =
    let tokens = AlgebraTokenizer.tokenize input
    let state = { Tokens = tokens; Position = 0 }
    
    let (left, state) = parseExpression state
    
    match currentToken state with
    | { Type = Equal } ->
        let state = advance state
        let (right, _) = parseExpression state
        Some { Left = left; Right = right }
    | _ -> None

let parseExpressionString (input: string) : Expr option =
    let tokens = AlgebraTokenizer.tokenize input
    let state = { Tokens = tokens; Position = 0 }
    let (expr, _) = parseExpression state
    Some expr
