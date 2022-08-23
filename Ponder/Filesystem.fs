module Ponder.Filesystem

open System.IO

type FilesystemFilter =
    | All
    | Sln
    | Csproj
    | Cs

type IFilesystem =
    abstract CurrentDirectory: string
    abstract ListFiles: directory: string -> filter: FilesystemFilter -> Async<string list>
    abstract LoadFile: path: string -> Async<string list>
    abstract Exists: path: string -> bool
    abstract SetCurrentDirectory: path: string -> unit
    
    
let private toWildcard filter =
    match filter with
    | All -> "*"
    | Sln -> "*.sln"
    | Csproj -> "*.csproj"
    | Cs -> "*.cs"
let realFilesystem = {
    new IFilesystem with
    member this.CurrentDirectory = System.Environment.CurrentDirectory
    member this.ListFiles path filter = async {
        return
            Directory.GetFiles(path, toWildcard filter)
            |> List.ofSeq
    }
    member this.LoadFile path = async {
        let! contents =
            File.ReadAllLinesAsync(path)
            |> Async.AwaitTask
            
        return contents |> List.ofSeq
    }
    member this.Exists path = File.Exists(path)
    member this.SetCurrentDirectory path = Directory.SetCurrentDirectory(path)
}
