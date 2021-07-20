module Counter

open Elmish
open Elmish.React
open Feliz
open Common
open Server

type State = { Count: int }

type Msg =
    | Increment
    | Decrement
    | DelayedIncrement

let init() = { Count = 0 }, Cmd.none

let update (counterMsg: Msg) (counterState: State) =
    match counterMsg with
    | Increment -> { counterState with Count = Count.add counterState.Count }, Cmd.none
    | Decrement -> { counterState with Count = counterState.Count - 1 }, Cmd.none
    | DelayedIncrement ->
        let delayed = async {
            do! Async.Sleep 2000
            return Increment
        }

        counterState, Cmd.fromAsync delayed

let render (state: State) (dispatch: Msg -> unit) =
   Html.div [
       Html.button [
           prop.onClick (fun _ -> dispatch Increment)
           prop.text "Increment"
       ]
       Html.button [
           prop.onClick (fun _ -> dispatch Decrement)
           prop.text "Decrement"
       ]
       Html.button [
           prop.onClick (fun _ -> dispatch DelayedIncrement)
           prop.text "Delayed Increment"
       ]
       divider
       Html.h1 state.Count
   ]
