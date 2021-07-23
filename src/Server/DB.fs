namespace Server

open System
open System.Data.SQLite

module DB =
    type WaterCounter =
        { Date: string
          ColdWater: int
          HotWater: int }

    module WaterCounter =
        let queryLatest n =
            let columns = "Date, ColdWater_liters, HotWater_liters"
            let order = sprintf " ORDER BY Date DESC LIMIT %d" n
            sprintf "SELECT %s FROM waterCounter %s;" columns order

        let translation (reader: SQLiteDataReader) =
            { Date = reader.["Date"].ToString();
            ColdWater = reader.["ColdWater_liters"].ToString() |> Convert.ToInt32;
            HotWater = reader.["HotWater_liters"].ToString() |> Convert.ToInt32 }
