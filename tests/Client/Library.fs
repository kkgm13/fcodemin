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
    // testCase "Fetched List of Meetings" <| fun _ ->
    //     let nt = null

        // Expect.equal nt null "Not null"
    testCase "Detected Info In Title" <| fun _ ->
        let expTitleName = "Meeting Testing"

        "input[name=Title]" << expTitleName

        let expTitle = someElement exp
        Expect.equal expTitle "Meeting Testing" "Unexpected"
]

let all =
    testList "All"
        [
            client
        ]

[<EntryPoint>]
let main _ = Mocha.runTests all
// Run: npm test