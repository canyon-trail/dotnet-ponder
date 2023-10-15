module Ponder.Program

open Microsoft.Extensions.Logging
open Ponder.AppState
open Ponder.AppStateReactor
open Ponder.SlnTypes
open Ponder.Filesystem
open Ponder.SlnParser
open FSharp.Control

// placeholder for logging
type Program = unit

type SlnFindResult =
    | Found of (string * SlnFile)
    | NotFound
    | ParseFail
    | Multiple of List<string>
    
let parseSlnFromPath (filesystem:IFilesystem) path = async {
    let! fileContents = filesystem.LoadFile path
    
    let sln = parseSlnFromLines fileContents
    
    return
        match sln.Projects with
        | [] -> ParseFail
        | _ -> Found (path, sln)
}
let findSln (filesystem: IFilesystem) = async {
    let! slnFiles = filesystem.ListFiles filesystem.CurrentDirectory Sln
    
    return!
        match slnFiles with
        | [f] -> parseSlnFromPath filesystem f
        | [] -> async.Return NotFound
        | _ -> Multiple slnFiles |> async.Return
}

let runReactor sln filesystem logFactory = async {
    let reactor = CompositionRoot.composeApp sln filesystem logFactory
    
    do! reactor.Finished |> Async.AwaitTask
}

let tryRunReactor (res: Result<string * SlnFile, string>) filesystem (loggerFactory: ILoggerFactory) = async {
    let logger = loggerFactory.CreateLogger<Program>()
    match res with
    | Error err ->
        logger.LogCritical("{err}", err)
        return 1
    | Ok (path, sln) ->
        logger.LogInformation("Opening sln at {path}", path)
        let dir = System.IO.Path.GetDirectoryName(path)
        System.Environment.CurrentDirectory <- dir
        do! runReactor sln filesystem loggerFactory
        
        return 0
}

[<EntryPoint>]
let main args =
    async {
        do! Async.SwitchToThreadPool ()
        let parseResult = CliArgs.parse (List.ofSeq args)
        let! slnFindResult =
            parseResult.SlnFile
            |> Option.map (parseSlnFromPath realFilesystem)
            |> Option.defaultWith (fun () -> findSln realFilesystem)

        let res =
            match slnFindResult with
            | Found (path, sln) -> Ok (path, sln)
            | Multiple _ -> Error "Multiple sln files found"
            | NotFound -> Error "No sln files found"
            | ParseFail -> Error "Invalid or empty sln file"
            
        return! tryRunReactor res realFilesystem parseResult.LogFactory
    } |> Async.RunSynchronously
