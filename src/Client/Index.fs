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
        // Errors: string list // Server Error Handler
    }

type Msg =
    | GotHello of string
    | GotMeetings of Meeting list
    // | AddMeet
    | AddedMeet of Meeting

//Model Initializing
let init() =
    // Default Info to add
    let model : Model =
        {
            Hello = "Test" 
            // Input = "" // Needs Input 
            Meetings = []
            // Errors = []
        }
    // Get actual data
    let getHello() = Fetch.get<unit, string> Route.hello
    let getMeetings() = Fetch.get<unit, Meeting> Route.test
    let cmd1 = Cmd.OfPromise.perform getHello () GotHello
    // let cmd2 = Cmd.OfAsync.either getMeetings () GotMeetings GotError
    // let cmd2 = Cmd.OfPromise.perform getMeetings () GotMeetings
    model, cmd1

//Updating the Model for the view
let update msg model =
    match msg with
    | GotHello hello ->
        { model with Hello = hello }, Cmd.none
    | GotMeetings meet ->
        { model with Meetings = meet}, Cmd.none
    // | AddMeet -> 
    //     let meet = Meeting.create 
    | AddedMeet meet ->
        { model with Meetings = model.Meetings @ [ meet ]}, Cmd.none
    // | GotError ex ->
    //     {model with Errors = ex.Message :: model.Errors}, Cmd.none

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
        ]
    ]
