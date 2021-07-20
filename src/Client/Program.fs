module AppProgram

open Elmish
open Elmish.React

Program.mkProgram App.init App.update App.render
|> Program.withReactSynchronous "home-app"
|> Program.run
