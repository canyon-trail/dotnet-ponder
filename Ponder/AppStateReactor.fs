module Ponder.AppStateReactor

open System
open System.Collections
open System.Collections.Concurrent
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Reactive
open Microsoft.Extensions.Logging
open Ponder.AppState.Actions
open Ponder.AppState.States
open Ponder.AppState.Reducer

type Dispatch = Action -> unit

type Reactor = {
    Stream: IObservable<State>
    Dispatch: Dispatch
    Finished: Task<unit>
}

let shouldEnd action =
    match action with
    | End -> true
    | _ -> false

let initReactor (logger: ILogger) =
    let subject = Subject.behavior Uninitialized
    
    let tcs = TaskCompletionSource<unit>()

    let dispatch =
        fun (action: Action) ->
            logger.LogInformation("Current state: {state}", subject.Value)

            logger.LogInformation("Reducing {action}", action)

            let newState = reduce subject.Value action

            logger.LogInformation("New state: {state}", newState)

            subject.OnNext newState

            if shouldEnd action
            then tcs.SetResult()
            else ()

    {
        Stream = subject
        Dispatch = dispatch
        Finished = tcs.Task
    }
