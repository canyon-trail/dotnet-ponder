module Ponder.AppState.States

open Ponder.SlnTypes

type LoadedRecord = {
    Sln: SlnFile;
    Projects: List<Project>;
} 

type State =
    | Opening of SlnFile
    | Loaded of LoadedRecord
