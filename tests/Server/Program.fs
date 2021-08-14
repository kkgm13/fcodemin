module Test

open Expecto // Patronom
open Shared
open System

let server = testList "Server" [
    testCase "Message returned correctly" <| fun _ ->
        let expectedResult = "Hello from SAFE!"        
        let result = Server.getMessage()
        Expect.equal result expectedResult "Result should be ok"

    testCase "Meeting Storage List added" <| fun _ ->
        let result = Server.Storage().GetMeetings() // Apparently Calls to Empty
        let storage = Server.Storage()
        Expect.isEmpty result "Meeting List is Not Grabbed"
        storage.AddMeeting(Meeting.create "Event 1" (Once(DateTime(2022,03,16,15,0,0), TimeSpan.FromHours(1.0)))) |> ignore 
        Server.Storage().AddMeeting(Meeting.create "Event 2" (Once(DateTime(2021,10,29,15,0,0), TimeSpan.FromHours(1.0)))) |> ignore
        Expect.isTrue (storage.GetMeetings() |> Seq.exists (fun b -> )) "Nope"
        

    // testCase "Storage Meeting Added to Storage List" <| fun _ ->
    //     let (meet:SaveMeetingRequest) = {Title = "Test Meet1"; Schedule = ("Once" |> ignore; "2021-12-25T10:00" |> ignore; "20"|>ignore;)} // Quickly to check
    //     Server.Storage().AddMeeting(Meeting.create meet.Title meet.Schedule) |> ignore
    //     let result = ""
    //     Expect.equal [] [] "Nope"
]

[<EntryPoint>]
let main _ = runTests defaultConfig server
//Run: dotnet run -p tests/Server