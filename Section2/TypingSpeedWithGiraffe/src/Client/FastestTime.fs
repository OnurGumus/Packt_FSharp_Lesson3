module FastestTime
open Fable.PowerPack
open Fable.PowerPack

    module Model =
        type Time = int array
        let zeroTime = [|0;0;0;0|]

    module private Core =
        open Model
        open Elmish
        let update  (message:Time) (model : Time) = 
            message, Cmd.none

    
    module private View =
        open Elmish
        open Model
        open Fable.Core.JsInterop
        open Fable.Import.Browser
        open Fable.Helpers.React
        open Fable.Helpers.React.Props

        let loadCmd = 
            let p () =
                promise {
                   let! r = Fetch.fetch("/api/fastestTime") []
                   return! r.json<Time>()
                }
            Cmd.ofPromise p () (fun r-> r) (fun r-> zeroTime)
        let private viewTime (timer : Time) = 
            timer.[0..2]
                |> Array.map (fun s -> s.ToString("00"))
                |> String.concat ":"

        let root (model:Time) dispatch  = 
            div [][
                Pages.viewLink Pages.Page.Home "Compete"
                h1 [][str <| "Fastest Time is " + (model |> viewTime )]
            ]
          
    

    open Elmish
    let init () =
       Model.zeroTime, (View.loadCmd)

    let view = View.root

    let update =  Core.update

