module Ponder.AppState.Actions

open Ponder.SlnTypes

type Action =
    | ProjectsLoaded of List<Project>
