module Ponder.AppState.Actions

open Ponder.AppState.States
open Ponder.SlnTypes

type Action =
    | Initialize of State
    | ProjectsLoaded of List<Project>
    | End
