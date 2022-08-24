module Ponder.AppState

open SlnTypes

type OpeningRecord = {
    Sln: SlnFile;
    Projects: List<Project>;
}

type LoadedRecord = {
    Projects: List<Project>;
} 

type State =
    | Opening of SlnFile
    | Loaded of LoadedRecord
