module ReactErrorBoundary

open Fable.Core
open Fable.Import
open Fable.Helpers.React

type [<AllowNullLiteral>] InfoComponentObject =
    abstract componentStack: string with get

type ErrorBoundaryProps =
    { Inner : React.ReactElement
      ErrorComponent : React.ReactElement
      OnError : exn * InfoComponentObject -> unit }

type ErrorBoundaryState =
    { HasErrors : bool }

// See https://github.com/MangelMaxime/Fulma/blob/master/docs/src/Widgets/Showcase.fs
// See https://reactjs.org/docs/error-boundaries.html
type ErrorBoundary(props) =
    inherit React.Component<ErrorBoundaryProps, ErrorBoundaryState>(props)
    do base.setInitState({ HasErrors = false })

    override x.componentDidCatch(error, info) =
        let info = info :?> InfoComponentObject
        x.props.OnError(error, info)
        x.setState(fun _ _ -> { HasErrors = true })

    override x.render() =
        if (x.state.HasErrors) then
            x.props.ErrorComponent
        else
            x.props.Inner

let renderCatchSimple errorElement element =
    ofType<ErrorBoundary,_,_> { Inner = element; ErrorComponent = errorElement; OnError = fun _ -> () } [ ]

let renderCatchFn onError errorElement element =
    ofType<ErrorBoundary,_,_> { Inner = element; ErrorComponent = errorElement; OnError = onError } [ ]