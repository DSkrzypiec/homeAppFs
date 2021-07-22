module Counter

open System
open Elmish
open Elmish.React
open Feliz
open Feliz.Bulma
open Common

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
    UploadDate: string
    UploadColdWater: int
    UploadHotWater: int
    UploadEnergy: double
}

type Msg =
    | ChangeView
    | SetUploadDate of string
    | SetUploadColdWater of int
    | SetUploadHotWater of int
    | SetUploadEnergy of double
    | SubmitUpload

let init() = {
    CurrentView = PresentCounters
    WaterTable = initMockWater
    UploadDate = (DateTime.Today.ToString())
    UploadColdWater = 0
    UploadHotWater = 0
    UploadEnergy = 0.0 }, Cmd.none

let update (counterMsg: Msg) (counterState: State) =
    match counterMsg with
    | ChangeView ->
        let newView = if counterState.CurrentView = PresentCounters then UploadCounters else PresentCounters 
        { counterState with CurrentView = newView }, Cmd.none

    | SetUploadDate date -> 
        { counterState with UploadDate = date }, Cmd.none

    | SetUploadColdWater coldWater ->
        { counterState with UploadColdWater = coldWater }, Cmd.none

    | SetUploadHotWater hotWater ->
        { counterState with UploadHotWater = hotWater }, Cmd.none

    | SetUploadEnergy energy ->
        { counterState with UploadEnergy = energy }, Cmd.none

    | SubmitUpload ->
        let newWater = {
            Date = counterState.UploadDate
            ColdWater = counterState.UploadColdWater
            HotWater = counterState.UploadHotWater
        }
        {
            counterState with 
                CurrentView = PresentCounters
                WaterTable = List.append counterState.WaterTable [newWater]
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
                prop.onChange (double >> SetUploadEnergy >> dispatch)
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
