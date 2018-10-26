module Api

open System
open Akkling
open FSharp.Control.Tasks.V2.ContextInsensitive
open Microsoft.AspNetCore.Http
open Giraffe

type Command =
    | Get
    | Set of int list

let system = System.create "typing-speed" <| Configuration.defaultConfig()

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
            let! res = fastestActor <? Get
            return! json res next ctx
        }
let setFastest =
    fun (_ : HttpFunc) (ctx : HttpContext) ->
        task{
            let! state = ctx.BindJsonAsync<int list>()
            fastestActor <! Set state
            return Some ctx}

let api : HttpHandler =
      subRoute "/api"
        (choose [
            GET  >=> choose [
                route "/fastestTime" >=>  getFastest
            ]
            POST >=> choose [
                route "/fastestTime" >=> setFastest
            ]
        ])
let webApp rootPath : HttpHandler =
    choose [
        api
        GET >=> htmlFile (rootPath + "/wwwroot/index.html")
    ]
