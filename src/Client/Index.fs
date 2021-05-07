module Index

open Elmish
open Thoth.Fetch
open Shared

// Default Information Types
    // Model Instance
type Model =
    {
        Hello: string
        Meetings: Meeting list
        Input: string
        Errors: string list // Server Error Handler
    }

type Msg =
    | GotHello of string
    | GotMeetings of Meeting list   // Get Meetings from Storage/DB
    | SetInput of string            // HTML input
    | SaveMeeting                   // Save Meeting
    | LoadMeeting                   // Load Meeting
    | MeetingLoaded of Meeting      // Meeting Loaded
    | MeetingSaved of Meeting           // Meeting Saved
    | GotError of exn               // Server Error Handler

//Model Initializing
let init() =
    // Default Info to add
    let model : Model =
        {
            Hello = "Test"  // Sample text
            Input = ""   // Needs Input 
            Meetings = []   // Blank Meetings Data
            Errors = []     // Blank Errors
        }
    // Get actual data
    let getHello() = Fetch.get<unit, string> Route.hello
    // Get actual data, and fix it as a list to bypass JSON format
    let getMeetings() = Fetch.get<unit, Meeting list> Route.meeting
    
    // Load a specific Meeting only
    // let loadMeeting meetingId =
    //     let loadMeeting () = Fetch.get<unit, Meeting> (sprintf "/api/meeting/%i" meetingId)
    //     Cmd.OfPromise.perform loadMeeting () MeetingLoaded

    // Send user data to the Server
    let saveMeet meet = 
        // Tell server a POST request is coming
        let save meet = Fetch.post<SaveMeetingRequest,Meeting> (Route.meeting, meet)
        // Create the promise function to save
        Cmd.OfPromise.perform save meet MeetingSaved
        
    // Induct Command Modules
    // Get single Information passed
    let cmd1 = Cmd.OfPromise.perform getHello () GotHello
    // Get List Information Passed
    let cmd2 = Cmd.OfPromise.either getMeetings () GotMeetings GotError
    model, Cmd.batch([cmd1 ; cmd2])

//Updating the Model for the view
let update msg model =
    match msg with
    // Sample Hello
    | GotHello hello ->
        { model with Hello = hello }, Cmd.none
    // Get the Meetings from Storage (Possible todo: Get DB info)
    | GotMeetings meet ->
        { model with Meetings = meet}, Cmd.none // Need to separate each one out
    // HTML Input Value
    | SetInput value ->
        { model with Input = value}, Cmd.none
    // Save Meeting to the Server
    // | SaveMeeting ->
    //     let meet = Meeting.create model.Input
        // let cmd = Cmd.OfPromise.perform saveMeet () meet AddedMeet 
        // model, saveMeet meet // Correct call triggering issue
    // Load a single Meeting
    // | LoadMeeting customerId ->
    //     // test
    // Loaded a Single Meeting???
    // | MeetingLoaded meet -> 
    //    {model with }, Cmd.none
    | MeetingSaved meet ->
        { model with Meetings = model.Meetings @ [ meet ]}, Cmd.none
    | GotError ex ->
        {model with Errors = ex.Message :: model.Errors}, Cmd.none

// Set up the React Portion of HTML
open Fable.React
open Fable.React.Props

// Update and Render to the Client
let view model dispatch =
    // Main Container (ONLY 1 IS ALLOWED!!)
    div [ Style [ TextAlign TextAlignOptions.Center; Padding 40 ] ] [
        // Each inner is a variation of each section
        div [] [
            img [ Src "favicon.png" ]
            h1 [] [ str "fcodemin" ]
            h2 [] [ str model.Hello ]
        ]
        // Meeting List Variation Section
        div [] [
            ul [ Style [TextAlign TextAlignOptions.Left;] ] [
                for meet in model.Meetings do // Loop around a list collection on view
                    li [OnMouseEnter (fun _ -> ())] [ str meet.Title ] // Concatination needed.
                    ul [ Style [TextAlign TextAlignOptions.Left;] ] [
                        li [] [ str (meet.Start.ToLocalTime().ToString()) ]
                        li [] [ str (meet.Duration.ToString())]
                        li [] [ str (meet.Id.ToString())]
                    ]
            ]
            // Error List (will come out blank, but present an empty array)
            p [] [ str (model.Errors.ToString()) ]
        ]
        // Meeting Form Variation Section
        div [] [
            // Form is interesting due to possible conversion and implementation
            form [ Action "" ][
                // Label
                label [ HTMLAttr.Custom ("for", "title") ][ str "Meeting Name:" ]
                    // Text Input
                input [ 
                        Type "text"
                        Id "title"
                        Name "title"
                        Placeholder "Meeting Name" 
                    ]
                br [] 
                // Date Creation
                label [ HTMLAttr.Custom ("for", "date") ][ str "Meeting Date:" ]
                    // Date-Time-Local Input
                input [ 
                        Type "datetime-local"
                        Id "start"
                        Name "start"
                        Placeholder "Meeting Date" 
                    ]
                br [] 
                // Duration Creation
                label [ HTMLAttr.Custom ("for", "duration") ][ str "Meeting Duration:" ]
                    // Number Input
                input [ 
                        Type "number"
                        Id "duration"
                        Name "duration"
                        Placeholder "Duration (Hour)" 
                        Min 1
                    ]
                // Input Submit / Reset
                hr []
                input [ 
                    Type "submit"
                    Value "Submit" ]
                input [ Type "reset" ]
            ]
        ]
    ]
