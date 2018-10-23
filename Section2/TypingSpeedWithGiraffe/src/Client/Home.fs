module Home


module  Model =
    type Time = int list

    type Status =
        | Initial
        | JustStarted
        | Complete
        | Wrong
        | Correct

    type Message = 
        | Tick
        | StartOver
        | KeyPress
        | TextUpdated of string

    type TypingModel = { Time : Time; Status : Status; CurrentText : string; TargetText : string }

    let zeroTime = [0;0;0;0]


module private Core =
    open System
    open Elmish
    open Model
    let private updateTime (timer : Time) : Time =
        let t3 =  (timer.[3] + 1) |> float
        let t0 =  (t3/100./60.) |> Math.Floor
        let t1 =  (t3/100. - t0 * 60.) |> Math.Floor
        let t2 =  (t3 - t1 * 100.- t0 * 6000.) |> Math.Floor
        [int t0;int t1;int t2; int t3]

    let update startTimer stopTimer message (model : TypingModel) =
        match message with
        | Tick -> { model with Time = updateTime model.Time}, Cmd.none
        | StartOver ->  {model with Status = Initial; Time = zeroTime; CurrentText = ""} , stopTimer
        | KeyPress when model.Status = Initial -> { model with Status = JustStarted} , startTimer

        | TextUpdated text when model.Status <> Complete ->
            let model = {model with CurrentText = text}

            if model.CurrentText = model.TargetText then
                {model with Status = Complete} , stopTimer
            else if (let originTextMatch = model.TargetText.Substring(0, model.CurrentText.Length)  
                model.CurrentText = originTextMatch) then
                { model with Status = Correct}, Cmd.none
            else
                {model with Status = Wrong}, Cmd.none

        | _ -> model, Cmd.none

module private View =
    open Elmish
    open Model
    open Fable.Core.JsInterop
    open Fable.Import.Browser
    open Fable.Helpers.React
    open Fable.Helpers.React.Props

    let originText = document.querySelector("#origin-text p").innerHTML

    let startTimer =
        let sub dispatch =
             if !!(window?myInterval) |> isNull then
                let interval = window.setInterval  ((fun () -> dispatch Tick), 10, [])
                window?myInterval <- interval
        Cmd.ofSub sub

    let stopTimer : Cmd<Message>=
        let sub _ =
            window.clearInterval !!(window?myInterval)
            window?myInterval <- null
        Cmd.ofSub sub

    let private viewTime (timer : Time) =
        seq{    
                yield timer.[0] 
                yield timer.[1] 
                yield timer.[2]
        }
            |> Seq.map (fun s -> s.ToString("00")) 
            |> String.concat ":"

    let private error _ _ = div[][str "Rendering error"]

    let private getBorderColor = function
        | Initial  | JustStarted  -> "grey"
        | Correct -> "#65CCf3"
        | Wrong -> "#E95D0F"
        | Complete ->  "#429890"

    let root model dispatch =
         div [][
            div [Class "test-wrapper"; Style [BorderColor <| !!getBorderColor model.Status] ] [
                textarea [
                    Rows 6
                    Value <| !!model.CurrentText
                    Placeholder "The clock starts when you start typing"
                    OnChange (fun e -> dispatch (TextUpdated !!e.target?value))
                    OnKeyPress (fun e -> if e.target?value = "" then  dispatch KeyPress)
                ][]
            ]
            div [Class "meta"] [
                section [Id "clock"][
                    div [Class "timer"][model.Time |> viewTime |> str ]
                ]
                button  [
                    Id "reset"
                    OnClick (fun _ -> dispatch StartOver) 
                ][str "Start over"]
            ]
        ]

open Model
open Elmish
let init () = 
    {   Status = Initial; 
        CurrentText = ""; 
        TargetText = View.originText; 
        Time = zeroTime } , Cmd.none
  

let view = View.root

let update =  Core.update View.startTimer View.stopTimer