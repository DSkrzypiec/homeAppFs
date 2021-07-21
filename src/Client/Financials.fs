module Financials

open Elmish
open Elmish.React
open Feliz
open Common

type State = { Todo: int }

type Msg =
    | LoadFinancials

let init() = { Todo = 0 }, Cmd.none

let update (financialsMsg: Msg) (financialsState: State) =
    match financialsMsg with
    | LoadFinancials -> financialsState, Cmd.none

let render (state: State) (dispatch: Msg -> unit) =
    Html.div [
        divider
        Html.h1 "TODO: Financials"
    ]

