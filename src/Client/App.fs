module App

open Elmish
open Elmish.React
open Feliz
open Common

type Page =
    | Counter
    | InputText

type State = {
    Counter: Counter.State
    InputText: InputText.State
    CurrentPage: Page
}

type Msg =
    | CounterMsg of Counter.Msg
    | InputTextMsg of InputText.Msg
    | SwitchPage of Page

let init() =
    let counterState, counterCmd = Counter.init()
    let inputTextState, inputTextCmd = InputText.init()

    let initialState =
        { Counter = counterState;
        InputText = inputTextState;
        CurrentPage = Page.Counter }

    let initialCmd = Cmd.batch [
        Cmd.map CounterMsg counterCmd
        Cmd.map InputTextMsg inputTextCmd
    ]

    initialState, initialCmd

let update (msg: Msg) (state: State) =
    match msg with
    | CounterMsg counterMsg ->
        let updatedCounter, counterCmd = Counter.update counterMsg state.Counter
        let appCmd = Cmd.map Msg.CounterMsg counterCmd
        { state with Counter = updatedCounter }, appCmd

    | InputTextMsg textInputMsg ->
        let updatedInputText, updateInputTextCmd = InputText.update textInputMsg state.InputText
        { state with InputText = updatedInputText }, updateInputTextCmd

    | SwitchPage page ->
        { state with CurrentPage = page }, Cmd.none

let render (state: State) (dispatch: Msg -> unit) =
    match state.CurrentPage with
    | Page.Counter ->
        Html.div [
            Html.button [
                prop.onClick (fun _ -> dispatch (SwitchPage Page.InputText))
                prop.text "Show text input"
            ]

            divider
            Counter.render state.Counter (CounterMsg >> dispatch)
        ]

    | Page.InputText ->
        Html.div [
            Html.button [
                prop.onClick (fun _ -> dispatch (SwitchPage Page.Counter))
                prop.text "Show counter"
            ]

            divider
            InputText.render state.InputText (InputTextMsg >> dispatch)
        ]

