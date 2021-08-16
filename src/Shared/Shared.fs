namespace Shared
open System

/////////////////////////////
///  Main Shared Object between server and client 
/// (Meeting Model)
/////////////////////////////

/// Original V1 Meeting Model
// type Meeting = 
//     {
//         Id : Guid
//         Title : string
//         Start : DateTime
//         Duration : TimeSpan
//     }
//     // Model Validations
//     member this.HasErrors() =
//         // Title Var is Empty
//         if this.Title.Length = 0 then Some "No Meeting Title Provided."
//         // Start var provided is past the current date & time now
//         else if this.Start.CompareTo(DateTime.Now) > 0 then Some "This meeting can't happen"
//         // Start var is between any Start var and subsequent durations
//         else None

/////////////////////////////
// Alternate for Repeating Meetings (Schedule Sub Model)
/////////////////////////////
type Schedule =
    | Once of DateTime * TimeSpan
    | Repeatedly of DateTime * TimeSpan * TimeSpan

// Version 2 Meeting Module
type Meeting = 
    {
        Id : Guid
        Title : string
        Schedule : Schedule
    }
    // Model Validations
    member this.HasErrors() =
        // Title Var is Empty
        if this.Title.Length = 0 then Some "No Meeting Title Provided."
        // Start var is between any Start var and subsequent durations
        // else if this.Schedule.Start.Compare(DateTime.Now) > 0 
        else None

//////////////////////////////////
/// Meeting Module for specific functions
//////////////////////////////////
module Meeting = 
    // Validator Method
    let isValid meet  = (
        // If null or whitespace only
        String.IsNullOrEmpty meet.Title ||
        // If meeting date and time compared is anything greater than the current time
        match meet.Schedule with 
            | Once(start, duration) -> 
                DateTime.Compare(start,DateTime.Now) < 0
            | Repeatedly(start, duration, repetition) -> 
                // Match with other repeated dates
                DateTime.Compare(start,DateTime.Now) < 0
                        ) |> not
        
    // Add/Create method
    let create title schedule  = 
        {
            Id = Guid.NewGuid()
            Title = title
            // Start = start
            // Duration = duration
            Schedule = schedule
        }

    ///<summary>
    /// Meeting Conflict Checker
    /// </summary>
    /// <returns>True for any conflicts; else false</returns>
    let conflict m1 m2 = 
        match m1.Schedule with 
            | Once(start1, length1)->
                match m2.Schedule with
                Once(start2, length2) ->
                        // failwith "Meeting falls under a known meeting at that time"
                        // start2 starts after start1 & start2 starts before start1 and its length
                    start2 > start1 && start2 < start1 + length1 || start2 + length2 > start1 && start2 < start1 + length1
                | Repeatedly(start2, length2, repitition2) ->
                    // failwith "Meeting falls under a known repeated meeeting"
                    start2 > start1 && start2 < start1 + length1 || start2 + length2 > start1 && start2 < start1 + length1
            | Repeatedly(start1, length1, repetition1)->
                match m2.Schedule with
                Once(start2, length2) ->
                        // failwith "Meeting falls under a known meeting at that time"
                    start2 > start1 && start2 < start1 + length1 || start2 + length2 > start1 && start2 < start1 + length1
                | Repeatedly(start2, length2, repitition2) ->
                    // failwith "Meeting falls under a known repeated meeeting"
                    start2 > start1 && start2 < start1 + length1 || start2 + length2 > start1 && start2 < start1 + length1
    ///<summary>
    /// Meeting Conflict Starter 
    /// </summary>
    /// <returns>Checks for anything in the list that is </returns>
    let conflictAny meet meetList =
        List.exists (fun e -> conflict meet e) meetList

//////////////////////////////////
///  Routes for Server
//////////////////////////////////
module Route =
    let hello = "/api/hello"
    let meeting = "/api/meetings"
    let hellos = "/api/hellos"
    // let meetSelect = "/api/meetings/"

//////////////////////////////////
/// Provide Data passing between the Client ot Server
//////////////////////////////////
type SaveMeetingRequest = {
    Title : string
    Schedule: Schedule
}