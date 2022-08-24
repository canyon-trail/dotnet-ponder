module Ponder.Program

open Ponder.SlnTypes
open Ponder.Filesystem
open Ponder.SlnParser

type SlnFindResult =
    | Found of SlnFile
    | NotFound
    | ParseFail
    | Multiple of List<string>
    
let parseSlnFromPath (filesystem:IFilesystem) path = async {
    let! fileContents = filesystem.LoadFile path
    
    let sln = parseSlnFromLines fileContents
    
    return
        match sln.Projects with
        | [] -> ParseFail
        | _ -> Found sln
}
let findSln (filesystem: IFilesystem) = async {
    let! slnFiles = filesystem.ListFiles filesystem.CurrentDirectory Sln
    
    return!
        match slnFiles with
        | [f] -> parseSlnFromPath filesystem f
        | [] -> async { return NotFound }
        | _ -> async { return Multiple slnFiles }
}

[<EntryPoint>]
let main args =
    do printf "starting"
    async {
        do! Async.SwitchToThreadPool ()
        let! slnFindResult =
            if args.Length = 0
            then findSln realFilesystem
            else parseSlnFromPath realFilesystem args[0]
            
                
        do
            match slnFindResult with
            | Found sln -> printfn $"Found sln: {args[0]}"
            | Multiple _ -> printfn "Multiple sln files found"
            | NotFound -> printfn "No sln files found"
            | ParseFail -> printfn "Invalid or empty sln file"
            
        return 0
    } |> Async.RunSynchronously
