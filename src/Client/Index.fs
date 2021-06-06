module Index

open Elmish
open Thoth.Fetch
open Shared
// open System

//////////////////////////////////////
/// Default Model Instance Type
//////////////////////////////////////
type Model =
    {
        Hello: string
        Meetings: Meeting list  // List of Meetings
        // All HTML Input Tags MUST HAVE THEIR OWN DEDICATED Model
        TitleInput: string       // Input Setter
        StartInput: string       // Input Setter
        DurationInput: int       // Input Setter
        Errors: string list // Server Error Handler
        TheMeeting: Meeting option
    }

//////////////////////////////////////
/// Default Model Message Instance
//////////////////////////////////////
type Msg =
    | GotHello of string
    // Data Grabbing
    | LoadMeeting of Meeting            // Get Specified Meeting from Storage/DB
    | LoadMeetings of Meeting list      // Get Meetings from Storage/DB
    | MeetingLoaded of Meeting          // Loaded Meetings 
    // All HTML Input Tags MUST HAVE THEIR OWN DEDICATED MESSAGE
    | SetTitleInput of string            // HTML Title input
    | SetStartInput of string            // HTML DateTime input
    | SetDurationInput of int            // HTML Number input
    // Sending to Server
    | SaveMeeting of SaveMeetingRequest                   // Save Meeting
    | MeetingSaved of Meeting           // Meeting Saved
    // Error Collection
    | GotError of exn               // Server Error Handler

//////////////////////////////////////
/// Default Model Initializations
//////////////////////////////////////
let init() =
    // Default Model Info to add
    let model =
        {
            Hello = "Test"          // Sample text
            TitleInput = ""         // Title Input
            StartInput = ""         // Start Input
            DurationInput = 0       // Duration Input
            Meetings = []           // Blank Meetings Data
            Errors = []             // Blank Errors
            TheMeeting = None       // Blank Meeting data?
        }

    // Get actual data
    let getHello() = Fetch.get<unit, string> Route.hello
    // Get actual data, and fix it as a list to bypass JSON format
    let loadMeetings() = Fetch.get<unit, Meeting list> Route.meeting

    // Induct Command Modules on load
    // Get single Information passed
    let cmd1 = Cmd.OfPromise.perform getHello () GotHello
    // Get List Information Passed
    let cmd2 = Cmd.OfPromise.either loadMeetings () LoadMeetings GotError
    model, Cmd.batch([cmd1 ; cmd2])

// Load a single Meeting
let loadMeeting meetId =
    let loadMeeting () = Fetch.get<unit, Meeting> (sprintf "/api/meeting/%s" meetId)
    Cmd.OfPromise.perform loadMeeting () MeetingLoaded

// Send user data to the Server
let saveMeeting meet = 
    printf "%s" (meet.ToString())
    // Tell server a POST request is coming
    let save meet = Fetch.post<SaveMeetingRequest,Meeting> (Route.meeting, meet)
    // Create the promise function to save
    Cmd.OfPromise.either save meet MeetingSaved GotError

//Updating the Model for the view
let update msg model =
    match msg with
    // Sample Hello
    | GotHello hello ->
        { model with Hello = hello }, Cmd.none

    /// <summary>
    /// Get the Meetings from Storage (Possible todo: Get DB info)
    /// </summary>
    /// <returns>List of Meetings</returns>
    | LoadMeetings meet ->
        { model with Meetings = meet}, Cmd.none

    /// <summary>
    /// Confirm Meeting has been saved
    /// </summary>
    /// <returns>Meeting List</returns>
    | MeetingSaved meetId ->
        // {model with GeneratedId = Some meetId; Message = "Saved Meetings"}, Cmd.none //????
        { model with Meetings = model.Meetings @ [ meetId ]}, Cmd.none

    /// <summary>
    /// Get the Selected Meeting from Storage (Possible todo: Get DB info)
    /// </summary>
    /// <returns>Selected Meeting</returns>
    | LoadMeeting meetId ->
        let id = meetId.ToString()
        model, loadMeeting id

    /// <summary>
    /// Get the Meetings from Storage (Possible todo: Get DB info)
    /// </summary>
    /// <returns>List of Meetings</returns>
    | MeetingLoaded meet->
        { model with TheMeeting = Some meet }, Cmd.none

    /// <summary>
    /// Set the Value for Title
    /// </summary>
    /// <returns>HTML Input Value for Title</returns>
    | SetTitleInput value ->
        printfn "Value for Input: %s" model.TitleInput // Debug to the Browser Console
        { model with TitleInput = value}, Cmd.none

    /// <summary>
    /// Print the Value for Start
    /// </summary>
    /// <returns>HTML Input Value for Title</returns>
    | SetStartInput value ->
        printfn "Value for Input: %s" model.StartInput // Debug to the Browser Console
        { model with StartInput = value}, Cmd.none

    /// <summary>
    /// Print the Value for Duration
    /// </summary>
    /// <returns>HTML Input Value for Title</returns>
    | SetDurationInput value ->
        printfn "Value for Input: %s" (model.DurationInput.ToString()) // Debug to the Browser Console
        { model with DurationInput = value}, Cmd.none

    /// <summary>
    /// Save the Meeting???
    /// </summary>
    /// <returns>???</returns>
    | SaveMeeting request ->
        model, saveMeeting request

    /// <summary>
    /// Get any errors found in the system
    /// </summary>
    /// <returns>List of Errors if any else empty array</returns>
    | GotError ex ->
        { model with Errors = ex.Message :: model.Errors }, Cmd.none

/////////////////////////////////////////////////////
/// Set up the React Portion of HTML
///////////////////////////////////////////////////// 
open Fable.React
open Fable.React.Props

///////////////////////////////////
/// HEADER COMPONENT VIEW FUNCTION
///////////////////////////////////
let topSection model =
    div [Style [ TextAlign TextAlignOptions.Center; Padding 40 ]] [
        a [Href "/"][
            img [ Src "favicon.png" ]
        ]
        h1 [] [ str "fcodemin" ]
        h2 [] [ str model.Hello ]
    ]

///////////////////////////////////
/// MEETING LIST COMPONENT VIEW FUNCTION
///////////////////////////////////
let meetList model =
    // Meeting List Variation Section
    div [Class "col-8"] [
        ul [ Style [TextAlign TextAlignOptions.Left;] ] [
            // Loop around the Meetings list collection on view
            for meet in model.Meetings do
                li [OnMouseEnter (fun _ -> ())] [ str meet.Title ]
                ul [ Style [TextAlign TextAlignOptions.Left;] ] [
                    li [] [ str (meet.Start.ToLocalTime().ToString()) ]
                    li [] [ str (meet.Duration.ToString())]
                    li [] [ str (meet.Id.ToString())]
                ]
        ]
        // Error List (will come out blank, but present an empty array)
        p [] [ 
            for err in model.Errors do
                str err
        ]
    ]

///////////////////////////////////
/// FORM COMPONENT VIEW FUNCTION
///////////////////////////////////
let meetForm model dispatch = 
    div [ Class "col-4" ] [
        // Form is interesting due to the conversion
        div [  ] [
            // Label
            div [ Class "mb-3" ][
                label [ HTMLAttr.Custom ("for", "Title") 
                        Class "form-label"]
                    [ str "Meeting Name:" ]
                // Text Input
                input [ 
                    Value model.TitleInput
                    Type "text"
                    Id "Title"
                    Name "Title"
                    Placeholder "Meeting Name"
                    Class "form-control"
                    OnChange (fun e -> dispatch(SetTitleInput((e.target :?> Browser.Types.HTMLInputElement).value)))
                ]
            ]
            div [ Class "mb-3" ][
                // Date Creation
                label [ HTMLAttr.Custom ("for", "Start") 
                        Class "form-label" ][ str "Meeting Date:" ]
                    // Date-Time-Local Input
                input [ 
                    Value model.StartInput
                    Type "datetime-local"
                    Id "Start"
                    Name "Start"
                    Placeholder "Meeting Date" 
                    Class "form-control"
                    OnChange (fun e -> dispatch(SetStartInput((e.target :?> Browser.Types.HTMLInputElement).value)))
                    // Disabled true
                ]
            ]
            div [ Class "mb-3" ][
                // Duration Creation
                label [ HTMLAttr.Custom ("for", "Duration") 
                        Class "form-label" ][ str "Meeting Duration: (Minutes)" ]
                    // Number Input
                input [ 
                    Value model.DurationInput
                    Type "number"
                    Id "Duration"
                    Name "Duration"
                    Placeholder "Meeting Duration (Minutes)" 
                    Min 1
                    Class "form-control"
                    OnChange (fun e -> dispatch(SetDurationInput(int (e.target :?> Browser.Types.HTMLInputElement).value)))
                    // Disabled true
                ]
            ]
            // Input Submit / Reset
            hr []
            div [Class "row"][
                div [Class "col-md-6 col-sm-12 py-1 d-grid gap-2"][
                    // Form submit can't work so the use of Button needs to be used 
                    button [
                        Class "btn btn-success"
                        OnClick (fun e -> printf "%s" (e.initEvent.ToString()))
                        // OnClick (fun e -> dispatch (saveMeeting e.Value))
                        ] [                        // Type "submit"
                        str "Submit"
                        //

                    ]
                ]
                div [Class "col-md-6 col-sm-12 py-1 d-grid gap-2"][
                    input [
                        Type "reset" 
                        Class "btn btn-danger btn-block"
                    ]
                ]
            ]
        ]
    ]

///////////////////////////////////
/// CORE VIEW FUNCTION
///////////////////////////////////
let view model dispatch =
    // Main Container (ONLY 1 IS ALLOWED!!)
    div [ ] [
        // Each inner is a variation of each section
        topSection model
        // Bootstrap
        div [ Class "container" ][ 
            div [ Class "row" ] [
                //Meeting List
                meetList model
                // Meeting Form Variation Section
                meetForm model dispatch
            ]
        ]
    ]