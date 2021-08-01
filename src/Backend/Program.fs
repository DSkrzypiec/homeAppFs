module Backend.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks
open Giraffe
open Backend

let mockHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            do! Async.Sleep 5000
            return! json [ "CRAP"; "CRAP 2"; "CRAP 3"] next ctx
        }

let waterHandler n =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            do! Async.Sleep 2000
            let data = Sqlite.select (DB.WaterCounter.queryLatest n) DB.WaterCounter.translation

            match data with
            | Ok res ->
                let x = res
                        |> List.map (fun row -> sprintf "(%s, %d, %d)" row.Date row.ColdWater row.HotWater)
                        |> List.fold (fun x y -> x + ";" + y) ""
                return! json x next ctx

            | Error err -> return! text err next ctx
        }

let webApp =
    choose [
        GET >=>
            choose [
                route "/" >=> text "hello world"
                route "/ping" >=> json "pong"
                route "/crap" >=> mockHandler
                route "/water" >=> (waterHandler 10)
                routef "/water/%i" waterHandler
            ]
        setStatusCode 404 >=> text "Not Found" ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder : CorsPolicyBuilder) =
    builder
        .WithOrigins(
            "http://localhost:8080",
            "http://localhost:5000",
            "https://localhost:5001")
       .AllowAnyMethod()
       .AllowAnyHeader()
       |> ignore

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
    (match env.IsDevelopment() with
    | true  ->
        app.UseDeveloperExceptionPage()
    | false ->
        app .UseGiraffeErrorHandler(errorHandler))
            .UseHttpsRedirection()
        .UseCors(configureCors) 
        .UseStaticFiles()
        .UseGiraffe(webApp)

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore

let configureLogging (builder : ILoggingBuilder) =
    builder.AddConsole()
           .AddDebug() |> ignore

[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .UseContentRoot(contentRoot)
                    .UseWebRoot(webRoot)
                    .Configure(Action<IApplicationBuilder> configureApp)
                    .ConfigureServices(configureServices)
                    .ConfigureLogging(configureLogging)
                    |> ignore)
        .Build()
        .Run()
    0
