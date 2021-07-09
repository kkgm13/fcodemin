// // Learn more about F# at http://fsharp.org

// open System

// [<EntryPoint>]
// let main argv =
//     printfn "Hello World from F#!"
//     0 // return an integer exit code
open Expecto

let server = testList "Server" [
    testCase "Message returned correctly" <| fun _ ->
        let expectedResult = "Hello from SAFE!"        
        let result = Server.getMessage()
        Expect.equal result expectedResult "Result should be ok"

    testCase "Meeting Added" <| fun _
]

[<EntryPoint>]
let main _ = runTests defaultConfig server