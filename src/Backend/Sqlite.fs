namespace Backend

open System.Data.SQLite
open FSharp.Core

module Sqlite =
    let select (query: string) (translation: SQLiteDataReader -> 'T) =
        let conn = new SQLiteConnection(Configuration.connectionString)

        try
            conn.Open()
            let cmd = new SQLiteCommand(query, conn)
            let reader = cmd.ExecuteReader()

            let mutable rows = List.empty<'T>

            while reader.Read() do
                rows <- List.append rows [(translation reader)]

            conn.Close()
            Ok rows
        with
            | e -> Error (sprintf "Coulnd't query [%s]: %s" query e.Message)
