module Counter

open Elmish
open Elmish.React
open Feliz
open Feliz.Bulma
open Common
open Server.Counters

type MockWater = { Date: string; ColdWater: int; HotWater: int }

let initMockWater = [
    { Date = "2019-12-23"; ColdWater = 1231231; HotWater = 88283 }
    { Date = "2019-12-28"; ColdWater = 1231531; HotWater = 89283 }
    { Date = "2019-12-31"; ColdWater = 1231788; HotWater = 90283 }
]

type CounterView =
    | PresentCounters
    | UploadCounters

type State = {
    CurrentView: CounterView
    WaterTable: MockWater list
    UploadDate: string option
    UploadColdWater: int option
    UploadHotWater: int option
    UploadEnergy: string option
    UploadError: string option
}

type Msg =
    | ChangeView
    | SetUploadDate of string
    | SetUploadColdWater of int
    | SetUploadHotWater of int
    | SetUploadEnergy of string
    | SubmitUpload
    | HideUploadError

let init() = {
    CurrentView = PresentCounters
    WaterTable = initMockWater
    UploadDate = None
    UploadColdWater = None
    UploadHotWater = None
    UploadEnergy = None
    UploadError = None }, Cmd.none

let delayedHideUploadError (duration: int) (dispatch: Msg -> unit) =
    let delayedCmd = async {
        do! Async.Sleep duration
        dispatch HideUploadError
    }

    Async.StartImmediate delayedCmd

let update (counterMsg: Msg) (counterState: State) =
    match counterMsg with
    | ChangeView ->
        let newView = if counterState.CurrentView = PresentCounters then UploadCounters else PresentCounters
        { counterState with CurrentView = newView }, Cmd.none

    | SetUploadDate date ->
        { counterState with UploadDate = Some date }, Cmd.none

    | SetUploadColdWater coldWater ->
        { counterState with UploadColdWater = Some coldWater }, Cmd.none

    | SetUploadHotWater hotWater ->
        { counterState with UploadHotWater = Some hotWater }, Cmd.none

    | SetUploadEnergy energy ->
        { counterState with UploadEnergy = Some energy }, Cmd.none

    | HideUploadError ->
        { counterState with UploadError = None }, Cmd.none

    | SubmitUpload ->
        let validationResult = Counters.validateCountersUpload {
            Date = counterState.UploadDate
            ColdWater = counterState.UploadColdWater
            HotWater = counterState.UploadHotWater
            Energy = counterState.UploadEnergy
        }

        match validationResult with
        | Error errors ->
            { counterState with
                UploadError = Some (errors |> List.fold (fun a b -> a + "\n" + b) "") }, Cmd.ofSub (delayedHideUploadError 5000)
        | Ok counters ->
            let newWater = {
                Date = counters.Date
                ColdWater = counters.ColdWater
                HotWater = counters.HotWater
            }
            {
                counterState with
                    CurrentView = PresentCounters
                    WaterTable = List.append counterState.WaterTable [newWater]
                    UploadDate = None
                    UploadColdWater = None
                    UploadHotWater = None
                    UploadEnergy = None
            }, Cmd.none

let renderWaterTableRows (waterTableRows: MockWater list) =
    let header = seq {
            Html.tr [
                Html.th [prop.text "Date"]
                Html.th [prop.text "Cold Water [l]"]
                Html.th [prop.text "Hot Water [l]"]
            ]
        }

    let rows = seq {
        for row in waterTableRows do
            Html.tr [
                Html.td [ prop.text row.Date]
                Html.td [ prop.text row.ColdWater]
                Html.td [ prop.text row.HotWater]
            ]
        }
    Seq.append header rows

let renderUploadDateInput (dispatch: Msg -> unit) =
    Bulma.field.div [
        Bulma.label "Date"
        Bulma.control.div [
            Bulma.input.text [
                prop.required true
                prop.placeholder "Date in YYYY-MM-DD"
                prop.onChange (SetUploadDate >> dispatch)
            ]
        ]
    ]

let renderUploadHotWaterInput (dispatch: Msg -> unit) =
    Bulma.field.div [
        Bulma.label "Hot Water"
        Bulma.control.div [
            Bulma.input.number [
                prop.required true
                prop.placeholder "Hot water in liters"
                prop.onChange (int >> SetUploadHotWater >> dispatch)
            ]
        ]
    ]

let renderUploadColdWaterInput (dispatch: Msg -> unit) =
    Bulma.field.div [
        Bulma.label "Cold Water"
        Bulma.control.div [
            Bulma.input.number [
                prop.required true
                prop.placeholder "Cold water in liters"
                prop.onChange (int >> SetUploadColdWater >> dispatch)
            ]
        ]
    ]

let renderUploadEnergyInput (dispatch: Msg -> unit) =
    Bulma.field.div [
        Bulma.label "Energy"
        Bulma.control.div [
            Bulma.input.text [
                prop.required true
                prop.placeholder "Energy in kWh"
                prop.onChange (SetUploadEnergy >> dispatch)
            ]
        ]
    ]

let renderUploadSubmitButton (dispatch: Msg -> unit) =
    Bulma.field.div [
        Bulma.field.isGrouped
        Bulma.field.isGroupedCentered
        prop.children [
            Bulma.control.div [
                Bulma.button.button [
                    Bulma.color.isLink
                    prop.text "Submit"
                    prop.onClick (fun event ->
                        event.preventDefault()
                        dispatch SubmitUpload
                    )
                ]
            ]
        ]
    ]

let renderUploadForm (dispatch: Msg -> unit) =
    Html.form [
        prop.onSubmit (fun _ -> dispatch SubmitUpload)
        prop.children [
            renderUploadDateInput dispatch
            renderUploadHotWaterInput dispatch
            renderUploadColdWaterInput dispatch
            renderUploadEnergyInput dispatch
            renderUploadSubmitButton dispatch
        ]
    ]

let renderUploadError (state: State) (dispatch: Msg -> unit) =
    match state.UploadError with
    | Some errMsg ->
        Bulma.message [
            Bulma.color.isDanger
            prop.children [
                Bulma.messageHeader [
                    Html.p errMsg
                ]
            ]
        ]
    | None -> Html.none

let render (state: State) (dispatch: Msg -> unit) =
    match state.CurrentView with
    | PresentCounters ->
        Bulma.container [
            Html.div [
                prop.style [ style.marginBottom 20 ]
                prop.children [
                    Bulma.button.button [
                        prop.onClick (fun _ -> dispatch ChangeView)
                        prop.text "Upload new"
                    ]
                ]
            ]
            Html.div [
                prop.className "water-table"
                prop.children [
                    Html.h3 "Water"
                    Html.table [
                        prop.className "table"
                        prop.children (renderWaterTableRows state.WaterTable)
                    ]
                ]
            ]
        ]

    | UploadCounters ->
        Bulma.container [
            renderUploadError state dispatch
            Html.div [
                prop.style [ style.marginBottom 20 ]
                prop.children [
                    Bulma.button.button [
                        prop.onClick (fun _ -> dispatch ChangeView)
                        prop.text "Show counters"
                    ]
                ]
            ]
            renderUploadForm dispatch
        ]
