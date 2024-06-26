module Server

open Giraffe
open Saturn
open System
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks

open Shared

///////////////////////////////////
// Storage Type for Fake Data
///////////////////////////////////
type Storage () =
    // Storage Array
    let meetings = ResizeArray<Meeting>()
    // Meeting Getter (equiv of index-list)
    member __.GetMeetings () =
        List.ofSeq meetings
    // Meeting Add (equiv of CRUD create/store)
    // Note: x was used to allow the use of member usages 
    member x.AddMeeting (meet : Meeting) =
        // Check for multiple
        printf "%A" meet // %A = Any Fsharp Obj
        if Meeting.isValid meet then
            // If meeting does NOT conflict with the list, then
            if not (Meeting.conflictAny meet (x.GetMeetings())) then
                // Add the Meeting in
                meetings.Add meet
                Ok meet
            else Error "Meeting Conflicted"
        else Error "Invalid Meeting"

// Storage Variable
let storage = Storage()

// Single Meeting Loading (Not in use but was good practice)
let loadMeeting (meetId: string) next ctx = task {
    let meet = {
            Id = Guid.Parse(meetId)
            Title = "Event 1"
            // Start = (DateTime(2021,07,16,15,0,0))
            // Duration = (TimeSpan.FromHours(1.0))
            Schedule = Once(DateTime(2021,07,16,15,0,0), TimeSpan.FromHours(1.0))
        }
    return! json meet next ctx
}

// Storage Faker Data
storage.AddMeeting(Meeting.create "Event 1" (Once(DateTime(2022,03,16,15,0,0), TimeSpan.FromHours(1.0)))) |> ignore 
storage.AddMeeting(Meeting.create "Event 2" (Once(DateTime(2021,10,29,15,0,0), TimeSpan.FromHours(1.0)))) |> ignore
storage.AddMeeting(Meeting.create "Event 3" (Repeatedly(DateTime(2021,11,29,15,0,0), TimeSpan.FromHours(1.0) , TimeSpan.FromDays(7.0)))) |> ignore
// // This should be illegal after a certain time
storage.AddMeeting(Meeting.create "Event Negative 1" (Once(DateTime(2021,04,16,15,0,0), TimeSpan.FromHours(1.0)))) |> ignore 
storage.AddMeeting(Meeting.create "Event Negative 2" (Repeatedly(DateTime(2021,08,16,15,0,0), TimeSpan.FromHours(1.0), TimeSpan.FromDays(3.0)))) |> ignore 

/////////////////////////////
/// Meeting Saved
/////////////////////////////
let saveMeeting next (ctx: HttpContext) = task { // Explicit Call to HttpContext?
    // let ats = ctx.GetFormValue.ToString()
    // // printfn ats
    let! meeting = ctx.BindJsonAsync<SaveMeetingRequest>() // Aids with DateTime
    // do! Database.addMeeting meeting // Database giving issues despite from Giraffe accoding to docs
    let x = storage.AddMeeting(Meeting.create meeting.Title meeting.Schedule)
    return! Successful.OK x next ctx
}
// Get message Method (For Sample Testing)
let getMessage () = "Hello from SAFE!"     

/// HTTP Route 
let webApp =
    router {
        // pipe_through headerPipe
        // not_found_handler (text "404") // Not Hound Handler
        get Route.hellos (getMessage () |> json )           // Get Message for Testing
        get Route.hello (json "Hello World")                // Hello World Sample View
        get Route.meeting (json (storage.GetMeetings()))    // Index Callout
        getf "/api/meeting/%s" loadMeeting                  // Show? Callout?
        post "/api/meeting-sent" saveMeeting                // Create Callout
    }

///////////////////////////////////
/// Main Application Server Setup
///////////////////////////////////
let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        memory_cache
        use_static "public"
        use_json_serializer (Thoth.Json.Giraffe.ThothSerializer())
        use_gzip
    }

run app // REQUIRED TO RUN EVERYTHING
