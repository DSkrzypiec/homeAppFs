module App

open Elmish
open Elmish.React
open Feliz
open Feliz.Bulma
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

let renderTitle =
    Html.div [
        prop.className "AppTitle"
        prop.style [ style.fontWeight.bold ]
        prop.children [
            Html.h1 "Home App F#"
        ]
    ]

let renderNavbar (state: State) (dispatch: Msg -> unit) =
    Html.div [
        prop.className "navbar"
        prop.children [
            Html.a [
                if state.CurrentPage = Page.InputText then prop.style [ style.fontWeight 700 ]
                prop.onClick (fun _ -> dispatch (SwitchPage Page.InputText))
                prop.text "Input Text"
            ]
            Html.a [
                if state.CurrentPage = Page.Counter then prop.style [ style.fontWeight 700 ]
                prop.onClick (fun _ -> dispatch (SwitchPage Page.Counter))
                prop.text "Counter"
            ]
            Html.a [
                prop.onClick (fun _ -> dispatch (SwitchPage Page.Counter))
                prop.text "Documents"
            ]
        ]
    ]

let render (state: State) (dispatch: Msg -> unit) =
    match state.CurrentPage with
    | Page.Counter ->
        Html.div [
            renderTitle
            renderNavbar state dispatch
            divider
            Counter.render state.Counter (CounterMsg >> dispatch)
        ]

    | Page.InputText ->
        Html.div [
            renderTitle
            renderNavbar state dispatch
            divider
            InputText.render state.InputText (InputTextMsg >> dispatch)
        ]

