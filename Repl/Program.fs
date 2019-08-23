// Learn more about F# at http://fsharp.org

open System
open Tokenizer
let rec runRepl eval =
    let line = Console.ReadLine()
    if line = "exit" then ()
    else
        let result = eval line
        Console.WriteLine(result :> Object);
        runRepl eval

    

[<EntryPoint>]
let main argv =
    printfn "Enter an expression"
    runRepl (fun s -> tokenize s)
    0 // return an integer exit code
