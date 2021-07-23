namespace Server.Validation

type Result<'T> = Result<'T, string list>

module Validation =
    let join (result: Result<'T>) (input: 'U) (f: 'T -> 'U -> Result<'T>) =
        match result with
        | Ok res -> f res input
        | Error err -> Error err