module View
open Model
open Fable.Core.JsInterop
open Fable.Import.Browser
open Fable.Helpers.React
open Fable.Helpers.React.Props



// let view model dispatch =
//     root model dispatch
//     |> ReactErrorBoundary.renderCatchFn
//         (fun (error, info) ->
//             console.log("Failed to render" + info.componentStack, error))
//             (error model dispatch)
let root (model : Model) (dispatch  : Message -> unit) = div[][]
    
   
// let init  =
//     { Status = Initial; 
//         CurrentText = ""; 
//         TargetText = originText; 
//         Time = zeroTime}, Cmd.none
// open Pages
// let init2  (page : Page option)  =
//    HomePageModel ({ Status = Initial; 
//         CurrentText = ""; 
//         TargetText = originText; 
//         Time = zeroTime}), Cmd.none