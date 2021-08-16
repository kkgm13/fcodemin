module Test

open Expecto
open Shared
open System

let (meetingList:list<Meeting>) = [{Id = Guid.NewGuid();Title="Meet 1";Schedule=Once(DateTime(2021,10,23,10,0,0),TimeSpan.FromMinutes(90.0))}]

let server = testList "Server" [
    testCase "Message returned correctly" <| fun _ ->
        let expectedResult = "Hello from SAFE!"        
        let result = Server.getMessage()
        Expect.equal result expectedResult "Result should be ok"

    testCase "Meetings Added in Meeting Storage List" <| fun _ ->
        let result = Server.Storage().GetMeetings()
        let storage = Server.Storage()
        Expect.isEmpty result "Meeting List is Not Grabbed" // Should call empty
        storage.AddMeeting(Meeting.create "Event 1" (Once(DateTime(2022,03,16,15,0,0), TimeSpan.FromHours(1.0)))) |> ignore 
        Server.Storage().AddMeeting(Meeting.create "Event 2" (Once(DateTime(2021,10,29,15,0,0), TimeSpan.FromHours(1.0)))) |> ignore
        Expect.isTrue (storage.GetMeetings() |> Seq.exists (fun b -> b.Title = "Event 1" )) "Nope"        

    testCase "Meeting from User Input Added to Meeting Storage List" <| fun _ ->
        let (meet1:SaveMeetingRequest) = {Title = "Test Meet1"; Schedule = Once(DateTime.Parse "2021-12-25T10:00", TimeSpan.FromMinutes 20.0)}
        let storage = Server.Storage()
        storage.AddMeeting(Meeting.create meet1.Title meet1.Schedule) |> ignore
        Expect.isTrue (storage.GetMeetings() |> Seq.exists (fun e -> e.Title = "Test Meet1")) " Nope"
    
    testCase "Adding 2 Meeting of the same day to a known Populated meeting list" <| fun _ ->
        let storage = Server.Storage()
        storage.AddMeeting(Meeting.create "General Meeting" (Once(DateTime(2021,12,25,12,0,0),TimeSpan.FromMinutes(90.0)))) |> ignore
        let (meet1:SaveMeetingRequest) = {Title = "Test Meeting 1"; Schedule = Once(DateTime.Parse "2021-12-25T10:00", TimeSpan.FromMinutes 20.0)}
        storage.AddMeeting(Meeting.create meet1.Title meet1.Schedule) |> ignore
        Expect.isTrue (storage.GetMeetings() |> Seq.exists (fun e -> e.Title = "Test Meeting 1")) " Nope"
        let (meet2:SaveMeetingRequest) = {Title = "Test Meeting 2"; Schedule = Once(DateTime.Parse "2021-12-25T14:00", TimeSpan.FromMinutes 50.0)}
        storage.AddMeeting(Meeting.create meet2.Title meet2.Schedule) |> ignore
        Expect.isTrue (storage.GetMeetings() |> Seq.exists (fun e -> e.Title = "Test Meeting 2")) " Nope"

    // Conflict Testing
    // 10am meeting for 90min VS 9:55am meeting for 20min
    testCase "Incoming Meeting Conflict staring before known meeting" <| fun _ ->
        let (client:SaveMeetingRequest) = {Title="Incoming Meet1"; Schedule = Once(DateTime.Parse "2021-10-23T09:55", TimeSpan.FromMinutes 20.0)}
        let incoming1 = {Id= Guid.NewGuid(); Title = client.Title; Schedule = client.Schedule}
        Expect.isTrue (Meeting.conflictAny incoming1 meetingList) "Added Normally"

    // 10am meeting for 90min VS 11:20am meeting for 40min
    testCase "Incoming Meeting Conflict just at the end of a known meeting" <| fun _ ->
        let (client:SaveMeetingRequest) = {Title="Incoming Meet2"; Schedule = Once(DateTime.Parse "2021-10-23T11:20", TimeSpan.FromMinutes 40.0)}
        let incoming2 = {Id= Guid.NewGuid(); Title = client.Title; Schedule = client.Schedule}
        Expect.isTrue (Meeting.conflictAny incoming2 meetingList) "Added Normally"
    
    // // 10am meeting for 90min VS 10:30am meeting for 15min
    testCase "Incoming Meeting Conflict between a known meeting" <| fun _ ->
        let (client:SaveMeetingRequest) = {Title="Incoming Meet3"; Schedule = Once(DateTime.Parse "2021-10-23T10:30", TimeSpan.FromMinutes 15.0)}
        let incoming3 = {Id= Guid.NewGuid(); Title = client.Title; Schedule = client.Schedule}
        Expect.isTrue (Meeting.conflictAny incoming3 meetingList) "Added Normally"
    
    // // 10am meeting for 90min VS 09:30am meeting for 150min
    testCase "Incoming Meeting Conflict outside but has a known meeting clash" <| fun _ ->
        let (client:SaveMeetingRequest) = {Title="Incoming Meet4"; Schedule = Once(DateTime.Parse "2021-10-23T09:30", TimeSpan.FromMinutes 150.0)}
        let incoming4 = {Id= Guid.NewGuid(); Title = client.Title; Schedule = client.Schedule}
        Expect.isTrue (Meeting.conflictAny incoming4 meetingList) "Added Normally"
]

[<EntryPoint>]
let main _ = runTests defaultConfig server
//Run: dotnet run -p tests/Server