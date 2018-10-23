open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open System
open Akkling
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http

type Command =
    | Get
    | Set of int list
let system = System.create "basic-sys" <| Configuration.defaultConfig()

let fastestActor = spawnAnonymous system <| props(fun ctx ->
    let rec loop (state:int list) = actor {
        match! ctx.Receive() with
        | Set l when l.[3] < state.[3] -> return! loop l
        | Get -> 
            ctx.Sender()  <! state
            return! loop state
        | _ -> return! loop state
    }

    loop [0;0;0;1000000])

let getFastest =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
          return! async{
                 return! fastestActor <? Get
            } |> Async.StartImmediateAsTask
        }
let setFastest =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task{
            let! state = ctx.BindJsonAsync<int list>()
            fastestActor <! Set state
            return Some ctx}

let webApp =
        choose [
        // Filters for GET requests
        GET  >=> choose [
            route "/fastestTime" >=>  getFastest
        ]
        // Filters for POST requests
        POST >=> choose [
            route "/fastestTime" >=> setFastest
        ]
        // If the HTTP verb or the route didn't match return a 404
        RequestErrors.NOT_FOUND "Not Found"
    ]

let configureApp (app : IApplicationBuilder) =
    // Add Giraffe to the ASP.NET Core pipeline
    app.UseGiraffe webApp

let configureServices (services : IServiceCollection) =
    // Add Giraffe dependencies
    services.AddGiraffe() |> ignore

[<EntryPoint>]
let main _ =
    WebHostBuilder()
        .UseKestrel()
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .Build()
        .Run()
    0