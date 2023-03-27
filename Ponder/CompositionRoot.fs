module Ponder.CompositionRoot

open Microsoft.Extensions.Logging
open Ponder.AppStateReactor

type App = {
    Reactor: Reactor
}

let composeApp sln filesystem (logFactory: ILoggerFactory) =
    let logger = logFactory.CreateLogger<Reactor>()
    let reactor = initReactor (AppState.States.Opening sln) logger
    
    ProjectLoader.createLoader reactor filesystem |> ignore
    
    reactor
