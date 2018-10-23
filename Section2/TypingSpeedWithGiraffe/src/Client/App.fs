module App

open Elmish
open Elmish.Browser.Navigation
open Elmish.React
open Elmish.Debug
open Elmish.HMR



open Pages
open Elmish.Browser
open Elmish.ReactNative
open Main.Model

let urlUpdate (result:Page option) (model : Model)  : Model * Cmd<Message>=
    let m, cmd = Home.init()
    HomePageModel m, Cmd.map HomeMessage cmd

Program.mkProgram init Main.Core.update Main.View.root
|> Program.toNavigable Pages.urlParser urlUpdate
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
#endif
|> Program.run
