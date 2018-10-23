module Main


module Core =
    open Model
    open Elmish
    let update (message : Message) (model : Model) = 
        match message, model with
        | HomeMessage msg, HomePageModel m -> 
            let homeModel, cmd = Home.update msg m
            HomePageModel(homeModel), Cmd.map HomeMessage cmd
        | _ -> model, Cmd.none
