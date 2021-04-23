namespace Shared

open System

// Guessing Shared Object files between server and client
type Meeting = {
    Id : Guid
    Title : string
    Start : DateTime
    Duration : TimeSpan
}

// type Schedule =
//     | Once of DateTime * TimeSpan
//     | Repeatedly of DateTime * TimeSpan * TimeSpan

module Meeting = 
    // Validator Method
    let isValid meet  = (
        // If null or whitespace only
        String.IsNullOrWhiteSpace meet.Title ||
        // If The date and time compared is anything greater than the 
        DateTime.Compare(meet.Start,DateTime.Now) < 0) |> not
        
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
    // let builder typeName methodName =
    //     sprintf "/api/%s/%s" typeName methodName

type IMeetingsApi =
    { getMeetings : unit -> Meeting list 
        // async can throw an issue based on the Middleware implementaion
      addMeeting : Meeting -> Async<Meeting> }