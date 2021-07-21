module App

open Elmish
open Elmish.React
open Feliz
open Common

type Page =
    | Counter
    | Documents
    | Financials

type State = {
    Counter: Counter.State
    Documents: Documents.State
    Financials: Financials.State
    CurrentPage: Page
}

type Msg =
    | CounterMsg of Counter.Msg
    | DocumentsMsg of Documents.Msg
    | FinancialsMsg of Financials.Msg
    | SwitchPage of Page

let init() =
    let counterState, counterCmd = Counter.init()
    let documentsState, documentsCmd = Documents.init()
    let financialsState, financialsCmd = Financials.init()

    let initialState =
        { Counter = counterState;
        Documents = documentsState;
        Financials = financialsState;
        CurrentPage = Page.Counter }

    let initialCmd = Cmd.batch [
        Cmd.map CounterMsg counterCmd
        Cmd.map DocumentsMsg documentsCmd
        Cmd.map FinancialsMsg financialsCmd
    ]

    initialState, initialCmd

let update (msg: Msg) (state: State) =
    match msg with
    | CounterMsg counterMsg ->
        let updatedCounter, counterCmd = Counter.update counterMsg state.Counter
        let appCmd = Cmd.map Msg.CounterMsg counterCmd
        { state with Counter = updatedCounter }, appCmd

    | DocumentsMsg documentsMsg ->
        let updatedDocuments, documentsCmd = Documents.update documentsMsg state.Documents
        let appCmd = Cmd.map Msg.DocumentsMsg documentsCmd
        { state with Documents = updatedDocuments }, appCmd

    | FinancialsMsg financialsMsg ->
        let updatedFinancials, financialsCmd = Financials.update financialsMsg state.Financials
        let appCmd = Cmd.map Msg.FinancialsMsg financialsCmd
        { state with Financials = updatedFinancials }, appCmd

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
                if state.CurrentPage = Page.Counter then prop.style [ style.fontWeight 700 ]
                prop.onClick (fun _ -> dispatch (SwitchPage Page.Counter))
                prop.text "Counters"
            ]
            Html.a [
                if state.CurrentPage = Page.Documents then prop.style [ style.fontWeight 700 ]
                prop.onClick (fun _ -> dispatch (SwitchPage Page.Documents))
                prop.text "Documents"
            ]
            Html.a [
                if state.CurrentPage = Page.Financials then prop.style [ style.fontWeight 700 ]
                prop.onClick (fun _ -> dispatch (SwitchPage Page.Financials))
                prop.text "Financials"
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

    | Page.Documents ->
        Html.div [
            renderTitle
            renderNavbar state dispatch
            divider
            Documents.render state.Documents (DocumentsMsg >> dispatch)
        ]

    | Page.Financials ->
        Html.div [
            renderTitle
            renderNavbar state dispatch
            divider
            Financials.render state.Financials (FinancialsMsg >> dispatch)
        ]

