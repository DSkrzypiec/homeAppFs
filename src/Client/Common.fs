module Common

open Elmish
open Elmish.React
open Feliz

let divider =
    Html.div [
        prop.style [ style.margin 10 ]
    ]

module Cmd =
    let fromAsync (operation: Async<'Msg>) : Cmd<'Msg> =
        let delayedCmd (dispatch: 'Msg -> unit) : unit =
            let delayedDispatch = async {
                let! msg = operation
                dispatch msg
            }

            Async.StartImmediate delayedDispatch

        Cmd.ofSub delayedCmd
