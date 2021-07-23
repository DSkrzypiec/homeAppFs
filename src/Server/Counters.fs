namespace Server.Counters

open Server.Validation

type CounterValidationInput = {
    Date: string option
    ColdWater: int option
    HotWater: int option
    Energy: string option
}

type Counters = {
    Date: string
    ColdWater: int
    HotWater: int
    Energy: double
}

module Validators =
    let validateDate (result: Counters) (input: CounterValidationInput) =
        match input.Date with
        | None -> Error [ "Date field cannot be empty" ]
        | Some dateValue ->
            if dateValue.[4] <> '-' ||  dateValue.[7] <> '-'
            then Error [ "Date should be in YYYY-MM-DD format" ]
            else Ok { result with Date = dateValue }

    let validateHotWater (result: Counters) (input: CounterValidationInput) =
        match input.HotWater with
        | None -> Error [ "HotWater field cannot be empty "]
        | Some value -> Ok { result with HotWater = value }

    let validateColdWater (result: Counters) (input: CounterValidationInput) =
        match input.ColdWater with
        | None -> Error [ "ColdWater field cannot be empty "]
        | Some value -> Ok { result with ColdWater = value }

    let validateEnergy (result: Counters) (input: CounterValidationInput) =
        match input.Energy with
        | None -> Error [ "Energy field cannot be empty" ]
        | Some value -> 
            match System.Double.TryParse(value) with
            | true, energyNumber -> Ok { result with Energy = double energyNumber }
            | _ -> Error [ (sprintf "Coulnd't parse [%s] to double" value) ]

module Counters =
    let validateCountersUpload (input: CounterValidationInput) =
        Ok { Date = ""; ColdWater = 0; HotWater = 0; Energy = 0.0 }
        |> (fun counters -> Validation.join counters input Validators.validateDate)
        |> (fun counters -> Validation.join counters input Validators.validateHotWater)
        |> (fun counters -> Validation.join counters input Validators.validateColdWater)
        |> (fun counters -> Validation.join counters input Validators.validateEnergy)