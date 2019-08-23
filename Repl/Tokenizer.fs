module Tokenizer
open System
type Tokens = Integer of int | Plus of string | Error of string



let rec getDigit index (str:string) (value:string) =
    if index > str.Length-1 then
        Some (Integer (Int32.Parse(value)))
    else
    let cVal = str.[index]
    if Char.IsDigit(cVal) then
        let nvalue = value + cVal.ToString()
        getDigit (index + 1) str nvalue
    else if value <> "" then Some(Integer (Int32.Parse(value)))
    else None



let tokenize (str:string) =
    [getDigit 0 str ""]
   
    