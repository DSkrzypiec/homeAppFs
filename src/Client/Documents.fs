module Documents

open Elmish
open Elmish.React
open Feliz
open Common

type State = { DocumentId: int }

type Msg =
    | LoadDocuments

let init() = { DocumentId = 0 }, Cmd.none

let update (documentsMsg: Msg) (documentsState: State) =
    match documentsMsg with
    | LoadDocuments -> documentsState, Cmd.none

let render (state: State) (dispatch: Msg -> unit) =
    Html.div [
        divider
        Html.h1 "TODO: Documents"
    ]

