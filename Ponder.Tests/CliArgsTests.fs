module Ponder.Tests.CliArgsTests

open Microsoft.Extensions.Logging.Abstractions
open Ponder
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``no args``() =
    let actual = CliArgs.parse []
    
    let expected = {
        LogFactory = NullLoggerFactory.Instance
        SlnFile = None
    }
    
    actual |> should equal expected

[<Fact>]
let ``sln file``() =
    let actual = CliArgs.parse ["some-file.sln"]
    
    let expected = {
        LogFactory = NullLoggerFactory.Instance
        SlnFile = "some-file.sln" |> Some
    }
    
    actual |> should equal expected
    
[<Fact>]
let ``logger``() =
    let actual = CliArgs.parse ["--log=some-file.txt"]
    
    actual.SlnFile |> should equal None
    actual.LogFactory |> should not' (be NullLoggerFactory.Instance)

[<Theory>]
[<InlineData("sln-file.sln", "--log=some-file.txt")>]
[<InlineData("--log=some-file.txt", "sln-file.sln")>]
let ``both specified``(arg1, arg2) =
    let actual = CliArgs.parse [arg1; arg2]
    
    actual.SlnFile |> should not' (be None)
    actual.LogFactory |> should not' (be NullLoggerFactory.Instance)
