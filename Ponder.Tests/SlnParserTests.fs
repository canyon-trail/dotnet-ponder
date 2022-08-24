module Ponder.Parsers.Tests.SlnParserTests

open System.IO
open System.Reflection
open Xunit
open FsUnit.Xunit
open FParsec

open Ponder.SlnParser
open Ponder.SlnTypes

[<Fact>]
let ``parses dotnet-ponder.sln`` () =
    let stream = Assembly
                     .GetExecutingAssembly()
                     .GetManifestResourceStream(
                         "Ponder.Tests.SlnExamples.dotnet-ponder.sln")

    async {
        use rdr = new StreamReader(stream)
        let! lines = readLines rdr

        let result = parseSlnFromLines lines

        result |> should equal {
            Projects = [
                { Name = "Ponder.Old"; Path = "src\\Ponder.Old\\Ponder.Old.csproj"  };
                { Name = "Ponder.Tests"; Path = "Ponder.Tests\\Ponder.Tests.csproj"  };
                { Name = "Ponder"; Path = "Ponder\\Ponder.csproj" };
                { Name = ".circleci"; Path = ".circleci" };
                { Name = "Ponder.Parsers"; Path = "Ponder.Parsers\\Ponder.Parsers.fsproj" };
                { Name = "Ponder.Parsers.Tests"; Path = "Ponder.Parsers.Tests\\Ponder.Parsers.Tests.fsproj" };
            ]
        }
    }

let doParse p str =
    match run p str with
    | FParsec.CharParsers.ParserResult.Success(result, _, _) -> Some result
    | Failure _ -> None

[<Fact>]
let ``parses project line`` () =
    let input = "Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"Ponder\", \"Ponder\Ponder.csproj\", \"{D523B64F-9B92-45E7-AABA-8E60A2A0384A}\""

    let result = doParse pProject input

    result |> should equal (Some { Name = "Ponder"; Path = "Ponder\\Ponder.csproj" })

[<Fact>]
let ``parses project line header`` () =
    let input = "Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\")"

    let result = doParse pProjectHeader input

    result |> should equal (Some ())

[<Fact>]
let ``parses guid`` () =
    let input = "2150E333-8FDC-42A3-9474-1A3956D46DE8"

    let result = doParse pGuid input

    result |> should equal (Some ())

[<Fact>]
let ``parses brace-wrapped guid`` () =
    let input = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}"

    let result = doParse pBracedGuid input

    result |> should equal (Some ())
[<Fact>]
let ``parses quoted, brace-wrapped guid`` () =
    let input = "\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\""

    let result = doParse (pInQuotes pBracedGuid) input

    result |> should equal (Some ())
