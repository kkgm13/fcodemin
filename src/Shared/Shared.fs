namespace Shared

open System

// Guessing Shared Object files between server and client
type Meeting = {
    Id : Guid
    Title : string
    // Start : DateTime
    // Duration : TimeSpan
}

// type Schedule =
//     | Once of DateTime * TimeSpan
//     | Repeatedly of DateTime * TimeSpan * TimeSpan

module Meeting = 
    // Validator Method
    let isValid title  =
        // If null or whitespace only
        String.IsNullOrWhiteSpace title |> not
        // ,DateTime.Compare(start,DateTime.Now) > 0
    // Add/Create method
    let create title = 
        {
            Id = Guid.NewGuid()
            Title = title
            // Variable to pass???
            // Start = DateTime(2021,04,18,10,0,0)
            // Duration = TimeSpan.FromHours(3.0)
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
    let test = "/api/test"
    // let builder typeName methodName =
    //     sprintf "/api/%s/%s" typeName methodName

type IMeetingsApi =
    { getMeetings : unit -> Async<Meeting list>
      addMeeting : Meeting -> Async<Meeting> }




