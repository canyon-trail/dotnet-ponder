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

let reduce (state: State) (action: Action) =
    match state with
    | Loaded _ -> state
    | Opening sln -> reduceOpening state sln action
