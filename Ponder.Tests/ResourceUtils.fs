module Ponder.Parsers.Tests.ResourceUtils

open System.IO
open System.Reflection
open Ponder.SlnParser

let readResource name =
    let stream = Assembly
                    .GetExecutingAssembly()
                    .GetManifestResourceStream(name)

    async {
        use rdr = new StreamReader(stream)
        let! lines = readLines rdr
        
        return lines
    }
