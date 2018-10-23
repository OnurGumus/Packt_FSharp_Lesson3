module Pages
open Elmish.Browser.UrlParser

[<RequireQualifiedAccess>]
type Page =
    | Home
    | FastestTime

let toPath =
    function
    | Page.Home -> "/"
    | Page.FastestTime -> "/fastestTime"

/// The URL is turned into a Result.
let pageParser : Parser<Page -> Page,Page> =
    oneOf
        [ map Page.Home (s "")
          map Page.FastestTime (s "fastestTime") ]

let urlParser location = parsePath pageParser location