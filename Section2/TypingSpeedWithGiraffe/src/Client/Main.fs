module Main

module Model = 
    open Home.Model
    open Elmish
    type PageModel =
        | HomePageModel of TypingModel
        | FastestTimeModel

    type Message = 
        | HomeMessage of Home.Model.Message
        | FastestTime

    type Model = PageModel
    let init page =
       let model, cmd  = Home.init()
       HomePageModel model, Cmd.map HomeMessage cmd

module Core =
    open Model
    open Elmish
    let update (message : Message) (model : Model) = 
        match message, model with
        | HomeMessage msg, HomePageModel m -> 
            let homeModel, cmd = Home.update msg m
            HomePageModel homeModel, Cmd.map HomeMessage cmd
        | _ -> model, Cmd.none

module View =
    open Model
    let root (model : Model) (dispatch  : Message -> unit) = 
        match model with 
        | HomePageModel m ->
            let r = Home.view m (HomeMessage >> dispatch)
            r
        | _ -> failwith "not imp"
    