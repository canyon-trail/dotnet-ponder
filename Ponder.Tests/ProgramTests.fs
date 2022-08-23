module Ponder.Parsers.Tests.ProgramTests

open Xunit
open FsUnit.Xunit
open Ponder.Parsers.Tests.FakeFilesystem
open Ponder.Parsers.Tests.ResourceUtils
open Ponder.Program
open Ponder.SlnParser

[<Fact>]
let ``returns NotFound for no sln file found`` () =
    let filesystem = FakeFilesystem "C:\\somewhere"
    
    async {
        let! result = findSln filesystem

        result |> should equal NotFound
    }
    
[<Fact>]
let ``returns Found for single sln file`` () =
    let filesystem = FakeFilesystem "C:\\somewhere"
    
    async {
        let! slnContents = readResource "Ponder.Parsers.Tests.SlnExamples.dotnet-ponder.sln"
        do filesystem.AddFile "C:\\somewhere\\example.sln" slnContents
        
        let expectedSlnFile = parseSlnFromLines slnContents
        
        let! result = findSln filesystem
        result |> should equal (Found expectedSlnFile)
    }
    
[<Fact>]
let ``returns Multiple for multiple sln files`` () =
    let filesystem = FakeFilesystem "C:\\somewhere"
    let path1 = "C:\\somewhere\\example1.sln"
    let path2 = "C:\\somewhere\\example2.sln"
    
    async {
        let! slnContents = readResource "Ponder.Parsers.Tests.SlnExamples.dotnet-ponder.sln"
        do filesystem.AddFile path1 slnContents
        do filesystem.AddFile path2 slnContents
        
        let! result = findSln filesystem
        result |> should equal (Multiple [path1; path2])
    }
