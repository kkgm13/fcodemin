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
        if Meeting.isValid meet.Title then
            meetings.Add meet
            Ok()
        else Error "Invalid todo"


// Faking Data
let storage = Storage()
storage.AddMeeting(Meeting.create "Event 1" (DateTime(2021,04,16,15,0,0)) (TimeSpan.FromHours(1.0))) |> ignore
storage.AddMeeting(Meeting.create "Event 2" (DateTime(2021,04,29,15,0,0)) (TimeSpan.FromHours(1.0))) |> ignore
storage.AddMeeting(Meeting.create "Event 3" (DateTime(2021,04,30,15,0,0)) (TimeSpan.FromHours(1.0))) |> ignore

let meetApi = 
    {
        getMeetings = fun() -> async {
            return storage.GetMeetings()
        }
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
        get "/meetings/" (json meetApi) // Send everything???
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
