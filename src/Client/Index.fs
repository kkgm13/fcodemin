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
        // Input: string
        Errors: string list // Server Error Handler
    }

type Msg =
    | GotHello of string
    | GotMeetings of  list
    // | AddMeet
    | AddedMeet of Meeting
    | GotError of exn // Server Error Handler

//Model Initializing
let init() =
    // Default Info to add
    let model : Model =
        {
            Hello = "Test" 
            // Input = "" // Needs Input 
            Meetings = []
            Errors = []
        }
    // Get actual data
    let getHello() = Fetch.get<unit, string> Route.hello
    let getMeetings() = Fetch.get<unit, Meeting> Route.meeting
    // Get single Information passed
    let cmd1 = Cmd.OfPromise.perform getHello () GotHello
    // Get List Information Passed
    let cmd2 = Cmd.OfPromise.either getMeetings () GotMeetings GotError
    model, cmd1

//Updating the Model for the view
let update msg model =
    match msg with
    | GotHello hello ->
        { model with Hello = hello }, Cmd.none
    | GotMeetings meet -> // Possibly incorrect as it is a list
        { model with Meetings = meet}, Cmd.none
    // | AddMeet -> 
    //     let meet = Meeting.create 
    | AddedMeet meet ->
        { model with Meetings = model.Meetings @ [ meet ]}, Cmd.none
    | GotError ex ->
        {model with Errors = ex.Message :: model.Errors}, Cmd.none

// Set up the React Portion of HTML
open Fable.React
open Fable.React.Props

// Update and Render to the Client
let view model dispatch =
    div [ Style [ TextAlign TextAlignOptions.Center; Padding 40 ] ] [
        div [] [
            img [ Src "favicon.png" ]
            h1 [] [ str "fcodemin" ]
            h2 [] [ str model.Hello ]
            // li [] [ str model.Meetings ]
            // p [] [ str model.Errors ]
        ]
    ]
