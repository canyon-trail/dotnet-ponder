module Ponder.SlnParser

open System.Collections.Generic
open System.IO
open System.Threading.Tasks
open FParsec

type SlnProject = {
    Name: string;
    Path: string;
}

type SlnFile = {
    Projects: SlnProject list
}

let EmptySln = {
    Projects = [];
}

type UserState = unit
type Parser<'t> = Parser<'t, UserState>

let stringFromCharList (xs:char list) = System.String.Concat(Array.ofList xs)
let pNoQuote: Parser<_> = manySatisfy (fun c -> c <> '"')
let pNoNewline: Parser<_> = manySatisfy (fun c -> c <> '\n')
let pHexChar: Parser<_> = skipSatisfy isHex
let pHexString n: Parser<_> = parray n pHexChar >>% ()
let pHyphen: Parser<_> = skipChar '-'
let pBetween left p right: Parser<_> = between (pstring left) (pstring right) p
let pInQuotes p: Parser<_> = pBetween "\"" p "\""
let pInParens p: Parser<_> = pBetween "(" p ")"
let pInBraces p: Parser<_> = pBetween "{" p "}"
let pGuid: Parser<_> =
    pHexString 8
    >>. pHyphen
    >>. pHexString 4
    >>. pHyphen
    >>. pHexString 4
    >>. pHyphen
    >>. pHexString 4
    >>. pHyphen
    >>. pHexString 12
    >>% ()
let pBracedGuid: Parser<_> = pInBraces pGuid

let pProjectHeader: Parser<_> =
    pstring "Project"
    .>> pInParens (pInQuotes pBracedGuid)
    >>% ()

let pProject: Parser<_> =
    pProjectHeader
    .>> pstring " = "
    >>. sepBy (pInQuotes pNoQuote) (pstring ", ")
    |>> fun x -> { Name = x.Head; Path = x.Tail.Head }

// TODO: move to separate file
let readLines (rdr :StreamReader) = async {
    let mutable lines = List<string>()

    let! l = rdr.ReadLineAsync() |> Async.AwaitTask
    let mutable line = l |> Option.ofObj
    while line.IsSome do
        lines.Add(line |> Option.get)
        let! l = rdr.ReadLineAsync() |> Async.AwaitTask
        line <- l |> Option.ofObj

    return List.ofSeq lines
}

let private parseLines lines =
    lines
    |> List.map (runParserOnString pProject () "")
    |> List.map (
        fun x ->
            match x with
            | Success (res, _, _) -> res |> Some
            | _ -> None
    )
    |> List.collect Option.toList

let parseSlnFromLines lines =
    let projects = parseLines (List.ofSeq lines)

    { Projects = projects }

let parseSlnFromStream (str: Stream): Task<SlnFile> = task {
    use rdr = new StreamReader(str, System.Text.Encoding.UTF8)

    let! lines = readLines rdr

    return parseSlnFromLines lines
}
