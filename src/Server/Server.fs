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
    // Meeting Getter (equiv of index-list)
    member __.GetMeetings () =
        List.ofSeq meetings
    // Meeting Add (equiv of CRUD create/store)
    member __.AddMeeting (meet : Meeting) =
        // Check for multiple
        printf "%A" meet // %A = Any Fsharp Obj
        if Meeting.isValid meet then
        // && Meeting.conflictAny meet (Meeting.GetMeetings()) 
            
            // if Meeting.conflict meet then
                meetings.Add meet
                Ok meet
            // else Error "Meeting Conflicted"
        else Error "Invalid Meeting"

// Storage Template
let storage = Storage()

// Issue with loading single Meeting
let loadMeeting (meetId: string) next ctx = task {
    let meet = {
            Id = Guid.Parse(meetId)
            Title = "Event 1"
            Start = (DateTime(2021,07,16,15,0,0))
            Duration = (TimeSpan.FromHours(1.0))
        }
    return! json meet next ctx
}

// Faking Data
storage.AddMeeting(Meeting.create "Event 1" (DateTime(2022,03,16,15,0,0)) (TimeSpan.FromHours(1.0))) |> ignore 
storage.AddMeeting(Meeting.create "Event 2" (DateTime(2021,10,29,15,0,0)) (TimeSpan.FromHours(1.0))) |> ignore
storage.AddMeeting(Meeting.create "Event 3" (DateTime(2021,11,29,15,0,0)) (TimeSpan.FromHours(1.0))) |> ignore
// This should be illegal after a certain time
storage.AddMeeting(Meeting.create "Event Negative 1" (DateTime(2021,04,16,15,0,0)) (TimeSpan.FromHours(1.0)) ) |> ignore 
storage.AddMeeting(Meeting.create "Event Negative 2" (DateTime(2021,07,16,15,0,0)) (TimeSpan.FromHours(1.0)) ) |> ignore 

let saveMeeting next (ctx: HttpContext) = task { // Explicit Call to HttpContext?
    // let ats = ctx.GetFormValue.ToString()
    // // printfn ats
    let! meeting = ctx.BindJsonAsync<SaveMeetingRequest>() // Aids with DateTime
    // do! Database.addMeeting meeting // Database giving issues despite from Giraffe accoding to docs
    let x = storage.AddMeeting(Meeting.create meeting.Title meeting.Start meeting.Duration)
    return! Successful.OK x next ctx
}
let getMessage () = "Hello from SAFE!"     

let webApp =
    router {
        // pipe_through headerPipe
        // not_found_handler (text "404") // Not Hound Handler
        get Route.hellos (getMessage () |> json )
        get Route.hello (json "Hello World")
        get Route.meeting (json (storage.GetMeetings()))    // Index Callout
        getf "/api/meeting/%s" loadMeeting              // Show? Callout?
        post "/api/meeting-sent" saveMeeting                      // Create Callout
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
