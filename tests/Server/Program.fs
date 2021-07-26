// // Learn more about F# at http://fsharp.org

// open System

// [<EntryPoint>]
// let main argv =
//     printfn "Hello World from F#!"
//     0 // return an integer exit code
module Test

open Expecto

let server = testList "Server" [
    testCase "Message returned correctly" <| fun _ ->
        let expectedResult = "Hello from SAFE!"        
        let result = Server.getMessage()
        Expect.equal result expectedResult "Result should be ok"

    testCase "Meeting List grabbed" <| fun _ ->
        let expected = []
        let result = []
        Expect.equal result expected "Meeting can be grabbed"
]

[<EntryPoint>]
let main _ = runTests defaultConfig server
//Run: dotnet run -p tests/Server