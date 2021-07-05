// namespace Client.Tests

// module Say =
//     let hello name =
//         printfn "Hello %s" name

module Tests

open Fable.Mocha

let client = testList "Client" [
    testCase "Hello received" <| fun _ ->
        let hello = Index.sayHello "SAFE V3"

        Expect.equal hello "Hello SAFE V3" "Unexpected greeting"
]

let all =
    testList "All"
        [
            client
        ]

[<EntryPoint>]
let main _ = Mocha.runTests all