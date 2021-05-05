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
    | GotMeetings of Meeting list
    | SetInput of string
    | AddMeeting
    | MeetingSaved of Meeting
    | GotError of exn // Server Error Handler

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
    let addMeet meet = 
        let save meet = Fetch.post<SaveMeetingRequest,Meeting> (Route.meeting, meet)
        Cmd.OfPromise.perform save meet MeetingSaved
    // Get single Information passed
    let cmd1 = Cmd.OfPromise.perform getHello () GotHello
    // Get List Information Passed
    let cmd2 = Cmd.OfPromise.either getMeetings () GotMeetings GotError
    model, Cmd.batch([cmd1 ; cmd2])

//Updating the Model for the view
let update msg model =
    match msg with
    | GotHello hello ->
        { model with Hello = hello }, Cmd.none
    | GotMeetings meet ->
        { model with Meetings = meet}, Cmd.none // Need to separate each one out
    | SetInput value ->
        { model with Input = value}, Cmd.none
    | AddMeeting ->
        let meet = Meeting.create model.Input
        let cmd = Cmd.OfAsync.perform addMeet () meet AddedMeet 
        model, AddMeeting meet
        // { model with Input = ""}, cmd
    | MeetingSaved meet ->
        { model with Meetings = model.Meetings @ [ meet ]}, Cmd.none
    | GotError ex ->
        {model with Errors = ex.Message :: model.Errors}, Cmd.none

// Set up the React Portion of HTML
open Fable.React
open Fable.React.Props

// Update and Render to the Client
let view model dispatch =
    div [ Style [ TextAlign TextAlignOptions.Center; Padding 40 ] ] [
        // Each inner is a variation of each section
        div [] [
            img [ Src "favicon.png" ]
            h1 [] [ str "fcodemin" ]
            h2 [] [ str model.Hello ]
        ]
        // List of Meetings
        div [] [
            ul [ Style [TextAlign TextAlignOptions.Left;] ] [
                for meet in model.Meetings do // Loop around a list collection on view
                    li [] [ str meet.Title ] // Concatination needed.
                    ul [ Style [TextAlign TextAlignOptions.Left;] ] [
                        li [] [ str (meet.Start.ToLocalTime().ToString()) ]
                        li [] [ str (meet.Duration.ToString())]
                    ]
            ]
            // Error List (will come out blank, but present an empty array)
            p [] [ str (model.Errors.ToString()) ]
        ]
        // Add form
        div [] [
            // Form is interesting due to possible conversion and implementation
            form [ Action "" ][
                label [ HTMLAttr.Custom ("for", "title") ][ str "Meeting Name:" ]
                input [ 
                        Type "text"
                        Id "title"
                        Name "title"
                        Placeholder "Meeting Name" 
                    ]
                br [] 
                // Date Creation
                label [ HTMLAttr.Custom ("for", "date") ][ str "Meeting Date:" ]
                input [ 
                        Type "datetime-local"
                        Id "start"
                        Name "start"
                        Placeholder "Meeting Date" 
                    ]
                hr []
                input [ 
                    Type "submit"
                    Value "Submit" ]
                input [ Type "reset" ]
            ]
        ]
    ]
