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

let initReactor (state: State) (logger: ILogger) =
    let subject = Subject.behavior state
    
    let mutable pendingActions = ConcurrentBag<Action>()
    let l = Object()
    let tcs = TaskCompletionSource<unit>()
    logger.LogInformation("Initial state: {state}", state)
    
    let dispatch =
        fun a ->
            pendingActions.Add(a)
            logger.LogInformation("Dispatched {action}", a)
            
            lock l (
                fun () ->
                    let p = Interlocked.Exchange(ref pendingActions, ConcurrentBag<Action>())
                    let mutable actions = p |> List.ofSeq
                    
                    let mutable newState = subject.Value
                    logger.LogInformation("Current state: {state}", newState)
                    
                    while actions.Length > 0 do
                        let a = actions.Head
                        actions <- actions.Tail
                        logger.LogInformation("Reducing {action}", a)
                        newState <- reduce newState a
                        
                    logger.LogInformation("New state: {state}", newState)

                    subject.OnNext newState
                    
                    let endActionExists =
                        actions
                        |> Seq.exists shouldEnd
                        
                    if endActionExists
                    then tcs.SetResult()
                    else ()
            )
    
    {
        Stream = subject
        Dispatch = dispatch
        Finished = tcs.Task
    }
