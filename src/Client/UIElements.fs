module UIElements

open Feliz
open Feliz.Bulma

let renderFooter =
    Bulma.footer [
        prop.style [ style.display.inlineBlock ]
        prop.children [
            Html.p "HomeApp F# by"
            Html.a [
                prop.href "https://dskrzypiec.dev"
                prop.text "DSkrzypiec"
            ]
        ]
    ]

let navigationBarStyles =
    [
        style.position.sticky
        style.top 0
        style.backgroundColor "#333"
        style.color "white"
        style.fontSize 17
        style.textAlign.left
        style.paddingLeft 20
        style.paddingTop 10
    ]