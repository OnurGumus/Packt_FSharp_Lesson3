module Model
open Home.Model
open Elmish
type PageModel =
    | HomePageModel of TypingModel
    | FastestTimeModel of FastestTimeModel

type Message = 
    | HomeMessage of Home.Model.Message
    | FastestTime

/// The composed model for the application, which is a single page state plus login information

type Model = PageModel

let init page =
   HomePageModel ( Home.Model.init ""), Cmd.none