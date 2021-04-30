module Server

open Giraffe
open Saturn
open System

open Shared

type Storage () =
    // Storage Faker Arrays
    let meetings = ResizeArray<_>()
    // Getter
    member __.GetMeetings () =
        List.ofSeq meetings
    // Adder
    member __.AddMeeting (meet : Meeting) =
        // Check for multiple
        if Meeting.isValid meet then
            meetings.Add meet
            Ok()
        else Error "Invalid Meeting"


// Faking Data
let storage = Storage()
storage.AddMeeting(Meeting.create "Event 1" (DateTime(2021,05,16,15,0,0)) (TimeSpan.FromHours(1.0))) |> ignore 
storage.AddMeeting(Meeting.create "Event 2" (DateTime(2021,05,29,15,0,0)) (TimeSpan.FromHours(1.0))) |> ignore
storage.AddMeeting(Meeting.create "Event 3" (DateTime(2021,06,29,15,0,0)) (TimeSpan.FromHours(1.0))) |> ignore
// This should be illegal
storage.AddMeeting(Meeting.create "Event Negative" (DateTime(2021,04,16,15,0,0)) (TimeSpan.FromHours(1.0))) |> ignore 

// let loadMeeting meeting next ctx = task {
//     let meet = {Title = meeting}
//     return! json meet next ctx
// }

let meetApi = 
    {
        getMeetings = fun() -> storage.GetMeetings()
        addMeeting = 
        fun meet -> async {
            match storage.AddMeeting meet with
            | Ok () -> return meet
            | Error e -> return failwith e
        }
    }

let webApp =
    router {
        get Route.hello (json "Hello World")
        // get "/meetings/" (json (storage.GetMeetings())) 
        get Route.meeting (json (meetApi.getMeetings()))// Send everything???
        // getf "/api/meeting/%i" loadMeeting
    }


let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        memory_cache
        use_static "public"
        use_json_serializer (Thoth.Json.Giraffe.ThothSerializer())
        use_gzip
    }

run app
