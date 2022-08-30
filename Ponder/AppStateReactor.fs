module Ponder.AppStateReactor

open System
open System.Collections.Concurrent
open FSharp.Control.Reactive

type Dispatch = AppState.Action -> unit

type Reactor = {
    Stream: IObservable<AppState.State>
    Dispatch: Dispatch
}

let initReactor state =
    let subject = Subject.behavior state
    
    let pendingActions = ConcurrentBag<AppState.Action>()
    let l = Object()
    
    let dispatch =
        fun a ->
            pendingActions.Add(a)
            
            lock l (
                fun () ->
                    let actions =
                        pendingActions
                        |> List.ofSeq
                        
                    let newState =
                        actions
                        |> List.fold AppState.reduce subject.Value

                    subject.OnNext newState
            )
    
    {
        Stream = subject
        Dispatch = dispatch
    }
