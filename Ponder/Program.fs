module Ponder.Program

open Ponder.Filesystem
open Ponder.SlnParser

type SlnFindResult =
    | Found of SlnFile
    | NotFound
    | Multiple of List<string>
    
let parseSlnFromPath (filesystem:IFilesystem) path = async {
    let! fileContents = filesystem.LoadFile path
    return Found (parseSlnFromLines fileContents)
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
            if args.Length > 0
            then parseSlnFromPath realFilesystem args[0]
            else findSln realFilesystem
                
        do
            match slnFindResult with
            | Found sln -> printfn $"Found sln: {args[0]}"
            | Multiple _ -> printfn "Multiple sln files found"
            | NotFound -> printfn "No sln files found"
            
        return 0
    } |> Async.RunSynchronously
