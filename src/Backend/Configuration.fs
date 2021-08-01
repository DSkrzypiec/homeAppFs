namespace Backend

open FSharp.Data

module Configuration =
    [<Literal>]
    let ConfigFilePath = "Config.json"

    type ConfigT = JsonProvider<ConfigFilePath>
    let Config = ConfigT.Load ConfigFilePath

    let connectionString = Config.ConnectionString
