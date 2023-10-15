module Ponder.CompositionRoot

open Microsoft.Extensions.Logging
open Ponder.AppStateReactor
open Ponder.AppState.Actions

type App = {
    Reactor: Reactor
}

let composeApp sln filesystem (logFactory: ILoggerFactory) =
    let logger = logFactory.CreateLogger<Reactor>()
    let reactor = initReactor  logger
    
    ProjectLoader.createLoader reactor filesystem logger |> ignore

    logger.LogInformation("dispatching initial state")

    AppState.States.Opening sln
    |> Initialize
    |> reactor.Dispatch

    reactor
