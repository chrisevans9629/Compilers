module Tokenizer
open System
type Token<'a> = {value:'a;index:int;}

type Tokens = Integer of Token<int> | Plus of Token<string> | Error of string


let rec getDigit index (str:string) (value:string) =
    
    let cVal = if index > str.Length-1 then
                   str.[index]
                else
                    ' '
    if Char.IsDigit(cVal) then
        let nvalue = value + cVal.ToString()
        getDigit (index + 1) str nvalue
    else 
    if value <> "" then 
        Some(Integer ({value=Int32.Parse(value);index=index}))
    else None



let rec tokenlist (str:string) (index:int) =
    let dig = getDigit index str ""
    match dig with
    | Some r -> 
        match r with
        | Integer i -> r::tokenlist str i.index
        | Plus p -> r::tokenlist str p.index
        | Error _ -> [r]
        
    | None -> []

let tokenize (str:string) =
    tokenlist str 0
   
    