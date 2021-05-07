module Server

open Giraffe
open Saturn
open System
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks

open Shared

// Storage Type for Fake Data
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

// Storage Template
let storage = Storage()
// Issue with loading single Meeting
let loadMeeting (meetId: int) next ctx = task {
    let meet = {
        Title = "Event 1"
        Start = (DateTime(2021,07,16,15,0,0))
        Duration = (TimeSpan.FromHours(1.0))
        }
    return! json meet next ctx
}

// Faking Data
storage.AddMeeting(Meeting.create "Event 1" (DateTime(2021,07,16,15,0,0)) (TimeSpan.FromHours(1.0))) |> ignore 
storage.AddMeeting(Meeting.create "Event 2" (DateTime(2021,08,29,15,0,0)) (TimeSpan.FromHours(1.0))) |> ignore
storage.AddMeeting(Meeting.create "Event 3" (DateTime(2021,09,29,15,0,0)) (TimeSpan.FromHours(1.0))) |> ignore
// This should be illegal after a certain time
storage.AddMeeting(Meeting.create "Event Negative" (DateTime(2021,04,16,15,0,0)) (TimeSpan.FromHours(1.0))) |> ignore 

let saveMeeting next (ctx: HttpContext) = task { // Explicit Call to HttpContext?
    let! meeting = ctx.BindModelAsync<SaveMeetingRequest>()
    // do! Database.addMeeting meeting // Database giving issues despite from Giraffe
    return! Successful.OK "Saved Meeting" next ctx
}

// let meetApi = 
//     {
//         getMeetings = fun() -> storage.GetMeetings()
//         addMeeting = 
//         fun meet -> async {
//             match storage.AddMeeting meet with
//             | Ok () -> return meet
//             | Error e -> return failwith e
//         }
//     }

let webApp =
    router {
        // pipe_through headerPipe
        not_found_handler (text "404") // Not Hound Handler

        get Route.hello (json "Hello World")
        get Route.meeting (json (storage.GetMeetings()))    // Index Callout
        post Route.meeting saveMeeting                      // Create Callout
        // getf "/api/meeting/%i" loadMeeting              // Show? Callout?
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
