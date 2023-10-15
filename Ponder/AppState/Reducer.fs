module Ponder.AppState.Reducer

open Ponder.AppState.States
open Ponder.AppState.Actions

let private reduceOpening state sln action =
    match action with
    | ProjectsLoaded p ->
        Loaded {
            Sln = sln
            Projects = p
        }
    | _ -> state

let private reduceUninitialized state action =
    match action with
    | Initialize st -> st
    | _ -> state

let reduce (state: State) (action: Action) =
    match state with
    | Uninitialized -> reduceUninitialized state action
    | Loaded _ -> state
    | Opening sln -> reduceOpening state sln action
