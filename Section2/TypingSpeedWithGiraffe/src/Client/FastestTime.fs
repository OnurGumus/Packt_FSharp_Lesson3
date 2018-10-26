module FastestTime

    module Model =
        type Time = int array
        let zeroTime = [|0;0;0;0|]

    module private Core =
        open Model
        open Elmish
        let update  message (model : Time) = model, Cmd.none

    
    module private View =
        open Elmish
        open Model
        open Fable.Core.JsInterop
        open Fable.Import.Browser
        open Fable.Helpers.React
        open Fable.Helpers.React.Props
        let private viewTime (timer : Time) =
            timer.[0..2]
                |> Array.map (fun s -> s.ToString("00"))
                |> String.concat ":"
        let root (model:Time) dispatch  = div [][str <| "Fastest Time is " + viewTime model]
    

    open Elmish
    let init () =
       Model.zeroTime, Cmd.none

    let view = View.root

    let update =  Core.update

