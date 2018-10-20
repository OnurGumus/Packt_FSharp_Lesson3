module Model

type Time = int array
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