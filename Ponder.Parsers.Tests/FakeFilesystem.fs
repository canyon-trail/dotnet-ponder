module Ponder.Parsers.Tests.FakeFilesystem

open Ponder.Parsers.Filesystem

type FakeFilesystem(root: string) =
    let mutable _root = root
    let mutable _files: Map<string, string list> = Map.empty
    
    member this.AddFile path contents =
        _files <- Map.add path contents _files
    
    interface IFilesystem with
        member this.CurrentDirectory = _root
        member this.ListFiles path filter =
            async {
                return _files
                    |> Map.keys
                    |> Seq.filter (fun x -> x.StartsWith(path))
                    |> List.ofSeq
            }
        member this.LoadFile path = async {
            return _files[path]
        }
        member this.Exists path = Map.containsKey path _files
        member this.SetCurrentDirectory path = _root <- path
