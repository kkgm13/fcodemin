// namespace Client.Tests

// module Say =
//     let hello name =
//         printfn "Hello %s" name

module Tests

open Fable.Mocha

let sample = testList "Sample" [
    testCase "Sample: Hello received" <| fun _ ->
        let hello = Index.sayHello "SAFE V3"

        Expect.equal hello "Hello SAFE V3" "Unexpected greeting"
]

let client = testList "Client" [
    testCase "Check Form Input is what was added: Title" <| fun _ ->
        // Let m1 start the initialization (model)
        let m1, _ = Index.init()
        // Let m2 Do the update based on the message we give and the m1 model
        let m2, _ = Index.update (Index.SetTitleInput "test") m1
        Expect.equal m2.TitleInput "test" "Not matching Title Found"
    testCase "Check Form Input is what was added: Start" <| fun _ ->
        let m1, _ = Index.init()
        let m2, _ = Index.update (Index.SetStartInput "2021-08-09T14:47") m1
        Expect.equal m2.StartInput "2021-08-09T14:47" "Incorrect Date Found"
    testCase "Check Form Input is what was added: Duration" <| fun _ ->
        let m1, _ = Index.init()
        let m2, _ = Index.update (Index.SetDurationInput 50) m1
        Expect.equal m2.DurationInput 50 "Incorrect Duration Found"
    testCase "Check Form Input is what was added: IsRepeat" <| fun _ ->
        let m1, _ = Index.init()
        let m2, _ = Index.update (Index.SetRepeatValInput true) m1
        Expect.equal m2.RepeatValInput true "Not a repeat checked"
    testCase "Check Form Input is what was added: Repeatition" <| fun _ ->
        let m1, _ = Index.init()
        let m2, _ = Index.update (Index.SetRepeatValInput true) m1
        Expect.equal m2.RepeatValInput true "Not a repeat checked"
        let m3, _ = Index.update (Index.SetRepetitionInput 7) m2
        // m3 needs reference due to m2 model accepting the visibility condition of RepetitionInput
        Expect.equal m3.RepetitionInput 7 "m2 Model did not detect the Indicated Input"
]

let all =
    testList "All"
        [
            sample
            client
        ]

[<EntryPoint>]
let main _ = Mocha.runTests all
// Run: npm test