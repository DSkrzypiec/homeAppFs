module Counter

open Elmish
open Elmish.React
open Feliz
open Feliz.Bulma
open Common
open Server

type State = { Count: int; CrapText: string; CrapTextToPrint: string }

type Msg =
    | Increment
    | Decrement
    | DelayedIncrement
    | SetCrapText of string
    | SetCrapTextToPrint

let init() = { Count = 0; CrapText = ""; CrapTextToPrint = "" }, Cmd.none

let update (counterMsg: Msg) (counterState: State) =
    match counterMsg with
    | Increment -> { counterState with Count = Count.add counterState.Count }, Cmd.none
    | Decrement -> { counterState with Count = counterState.Count - 1 }, Cmd.none
    | SetCrapText text -> { counterState with CrapText = text }, Cmd.none
    | SetCrapTextToPrint -> { counterState with CrapTextToPrint = counterState.CrapText }, Cmd.none
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

       divider
       divider

       Html.div [
           prop.className "Form"
           prop.children [
               Html.label [
                   prop.text "Crap"
               ]
               Html.input [
                   prop.placeholder "Tu wpisz crap"
                   prop.onChange (SetCrapText >> dispatch)
               ]
               Bulma.button.button [
                   prop.onClick (fun _ -> dispatch SetCrapTextToPrint)
                   prop.text "Update crap text"
               ]
               Html.h1 state.CrapTextToPrint
           ]
       ]
   ]
