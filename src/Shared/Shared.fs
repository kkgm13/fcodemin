namespace Shared

open System

// Main Shared Object files between server and client (Model Spec)
type Meeting = 
    {
        Id : Guid
        Title : string
        Start : DateTime
        Duration : TimeSpan
    }
    //Giraffe Model Validations
    member this.HasErrors() =
        // Title Var is Empty
        if this.Title.Length = 0 then Some "No Meeting Title Provided."
        // Start var provided is past the current date & time now
        else if this.Start.CompareTo(DateTime.Now) > 0 then Some "This meeting can't happen"
        // Start var is between any Start var and subsequent durations
        else None

// type Schedule =
//     | Once of DateTime * TimeSpan
//     | Repeatedly of DateTime * TimeSpan * TimeSpan

module Meeting = 
    // Validator Method
    let isValid meet  = (
        // If null or whitespace only
        String.IsNullOrWhiteSpace meet.Title ||
        // If meeting date and time compared is anything greater than the current time
        DateTime.Compare(meet.Start,DateTime.Now) < 0
        // If an existing
                        ) |> not
        
    // Add/Create method
    let create title start duration= 
        {
            Id = Guid.NewGuid()
            Title = title
            Start = start
            Duration = duration
        }
    // Conflict Checker
    // let conflict m1 m2 = 
    //     match m1 with 
    //     | Once(start1, length1)->
    //         match m2 with
    //         Once(start2, length2) ->
    //             false
    //     | Repeatedly(start1, length1, repetition1)->
    //         failwith "Unable to provide meeting"

module Route =
    let hello = "/api/hello"
    let meeting = "/api/meetings"

// type IMeetingsApi =
//     { getMeetings : unit -> Meeting list 
//         // async can throw an issue based on the Middleware implementaion
//       addMeeting : Meeting -> Async<Meeting> }

// Provide Data passing between the Client ot Server
type SaveMeetingRequest = {
    Title : string
    Start : DateTime
    Duration : TimeSpan
}